export const CANVAS_WIDTH = 800;
export const CANVAS_HEIGHT = 450;
export const SCALE_MODES = new Set(['integer', 'native', 'fit']);
export const DEFAULT_SCALE_MODE = 'native';

export function normalizeScaleMode(mode) {
    if (!mode) {
        return DEFAULT_SCALE_MODE;
    }

    const normalized = String(mode).toLowerCase();
    return SCALE_MODES.has(normalized) ? normalized : DEFAULT_SCALE_MODE;
}

export function computeDisplaySize(mode, viewportWidth, viewportHeight, dpr = 1) {
    const safeDpr = Number.isFinite(dpr) && dpr > 0 ? dpr : 1;

    if (viewportWidth <= 0 || viewportHeight <= 0) {
        return {
            cssWidth: CANVAS_WIDTH / safeDpr,
            cssHeight: CANVAS_HEIGHT / safeDpr,
            deviceWidth: CANVAS_WIDTH,
            deviceHeight: CANVAS_HEIGHT,
            scale: 1,
            dpr: safeDpr
        };
    }

    // Convert viewport from CSS pixels to device pixels first so scaling is DPI/zoom aware.
    const viewportDeviceWidth = Math.max(1, Math.floor(viewportWidth * safeDpr));
    const viewportDeviceHeight = Math.max(1, Math.floor(viewportHeight * safeDpr));

    let deviceWidth;
    let deviceHeight;
    let scale;

    if (mode === 'native') {
        // 1 source pixel -> 1 physical device pixel.
        scale = 1;
        deviceWidth = CANVAS_WIDTH;
        deviceHeight = CANVAS_HEIGHT;
    } else if (mode === 'fit') {
        const fitRatio = Math.min(viewportDeviceWidth / CANVAS_WIDTH, viewportDeviceHeight / CANVAS_HEIGHT);
        const safeRatio = Number.isFinite(fitRatio) && fitRatio > 0 ? fitRatio : 1;
        scale = safeRatio;
        deviceWidth = Math.max(1, Math.floor(CANVAS_WIDTH * safeRatio));
        deviceHeight = Math.max(1, Math.floor(CANVAS_HEIGHT * safeRatio));
    } else {
        // integer mode
        const integerScale = Math.max(1, Math.floor(Math.min(
            viewportDeviceWidth / CANVAS_WIDTH,
            viewportDeviceHeight / CANVAS_HEIGHT
        )));
        scale = integerScale;
        deviceWidth = CANVAS_WIDTH * integerScale;
        deviceHeight = CANVAS_HEIGHT * integerScale;
    }

    return {
        cssWidth: deviceWidth / safeDpr,
        cssHeight: deviceHeight / safeDpr,
        deviceWidth,
        deviceHeight,
        scale,
        dpr: safeDpr
    };
}
