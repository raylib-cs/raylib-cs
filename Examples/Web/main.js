import { dotnet } from './_framework/dotnet.js'
import {
    CANVAS_WIDTH,
    CANVAS_HEIGHT,
    normalizeScaleMode,
    computeDisplaySize
} from './scaleUtils.js'

const status = document.getElementById('status');
const select = document.getElementById('examples');
const canvas = document.getElementById('canvas');
const viewport = document.getElementById('viewport');
const scaleModeSelect = document.getElementById('scaleMode');

function readScaleModeFromQuery() {
    const params = new URLSearchParams(window.location.search);
    return normalizeScaleMode(params.get('scale'));
}

function writeScaleModeToQuery(mode) {
    const params = new URLSearchParams(window.location.search);
    params.set('scale', mode);
    const query = params.toString();
    const nextUrl = query.length > 0 ? `${window.location.pathname}?${query}` : window.location.pathname;
    window.history.replaceState(null, '', nextUrl);
}

let scaleMode = readScaleModeFromQuery();
let Host = null;

function ensureBackingBufferSize() {
    if (canvas.width !== CANVAS_WIDTH) {
        canvas.width = CANVAS_WIDTH;
    }

    if (canvas.height !== CANVAS_HEIGHT) {
        canvas.height = CANVAS_HEIGHT;
    }
}

function updateScaleDiagnostics(displayScale, cssWidth, cssHeight, deviceWidth, deviceHeight, dpr) {
    status.textContent =
        `Running. Example: ${scaleMode} scaling | DPR ${dpr.toFixed(2)} | ` +
        `CSS ${cssWidth.toFixed(2)}x${cssHeight.toFixed(2)} | ` +
        `Device ${deviceWidth}x${deviceHeight} | Backing ${canvas.width}x${canvas.height} | ` +
        `Scale ${displayScale.toFixed(2)}`;
}

function syncMouseScale() {
    if (!Host) {
        return;
    }

    const cssWidth = canvas.clientWidth;
    const cssHeight = canvas.clientHeight;
    if (cssWidth <= 0 || cssHeight <= 0) {
        return;
    }

    // Map CSS/display mouse coords to the fixed backing buffer (canvas.width/height).
    const scaleX = canvas.width / cssWidth;
    const scaleY = canvas.height / cssHeight;
    Host.SetMouseScaleFromDisplay(scaleX, scaleY);
}

function applyDisplayScale() {
    ensureBackingBufferSize();

    const viewportWidth = viewport.clientWidth || CANVAS_WIDTH;
    const viewportHeight = viewport.clientHeight || CANVAS_HEIGHT;
    const dprRaw = window.devicePixelRatio ?? 1;
    const display = computeDisplaySize(scaleMode, viewportWidth, viewportHeight, dprRaw);

    canvas.style.width = `${display.cssWidth}px`;
    canvas.style.height = `${display.cssHeight}px`;

    // Center canvas inside viewport, leaving letterboxing around it.
    viewport.style.justifyContent = 'center';
    viewport.style.alignItems = 'center';

    syncMouseScale();

    updateScaleDiagnostics(
        display.scale,
        display.cssWidth,
        display.cssHeight,
        display.deviceWidth,
        display.deviceHeight,
        display.dpr
    );
}

function setScaleMode(mode, updateQuery) {
    const nextMode = normalizeScaleMode(mode);
    if (nextMode === scaleMode && !updateQuery) {
        return;
    }

    scaleMode = nextMode;
    if (scaleModeSelect) {
        scaleModeSelect.value = scaleMode;
    }

    if (updateQuery) {
        writeScaleModeToQuery(scaleMode);
    }

    applyDisplayScale();
}

if (scaleModeSelect) {
    scaleModeSelect.value = scaleMode;
    scaleModeSelect.addEventListener('change', () => {
        setScaleMode(scaleModeSelect.value, true);
    });
}

window.addEventListener('resize', applyDisplayScale);
if (window.visualViewport) {
    window.visualViewport.addEventListener('resize', applyDisplayScale);
}

applyDisplayScale();

const { getAssemblyExports, getConfig, runMain } = await dotnet
    .withDiagnosticTracing(false)
    .create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);
Host = exports.Examples.Web.Host;

// raylib's GLFW/emscripten backend renders into this canvas.
dotnet.instance.Module['canvas'] = canvas;

// Runs Host.Main() -> InitWindow + default example Init(). Must come after the canvas is bound.
await runMain();
applyDisplayScale();

// Populate the navigation dropdown from the registered examples.
select.innerHTML = '';
for (const name of Host.GetExampleNames().split('\n').filter(n => n.length > 0)) {
    const opt = document.createElement('option');
    opt.value = name;
    opt.textContent = name;
    select.appendChild(opt);
}
select.addEventListener('change', () => Host.SetExample(select.value));

setScaleMode(scaleMode, false);

// Drive raylib one frame per animation tick (never block the browser).
function mainLoop() {
    Host.UpdateFrame();
    requestAnimationFrame(mainLoop);
}
requestAnimationFrame(mainLoop);
