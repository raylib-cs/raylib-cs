import { describe, it } from 'node:test';
import assert from 'node:assert/strict';
import {
    CANVAS_WIDTH,
    CANVAS_HEIGHT,
    normalizeScaleMode,
    computeDisplaySize
} from './scaleUtils.js';

describe('normalizeScaleMode', () => {
    it('returns native for null, undefined, and empty string', () => {
        assert.equal(normalizeScaleMode(null), 'native');
        assert.equal(normalizeScaleMode(undefined), 'native');
        assert.equal(normalizeScaleMode(''), 'native');
    });

    it('normalizes allowlisted values case-insensitively', () => {
        assert.equal(normalizeScaleMode('INTEGER'), 'integer');
        assert.equal(normalizeScaleMode('Fit'), 'fit');
        assert.equal(normalizeScaleMode('native'), 'native');
    });

    it('returns native for unknown values', () => {
        assert.equal(normalizeScaleMode('stretch'), 'native');
        assert.equal(normalizeScaleMode('bogus'), 'native');
    });
});

describe('computeDisplaySize', () => {
    it('native mode uses 1:1 device pixels at scale 1', () => {
        const result = computeDisplaySize('native', 1200, 800, 1);

        assert.equal(result.scale, 1);
        assert.equal(result.deviceWidth, CANVAS_WIDTH);
        assert.equal(result.deviceHeight, CANVAS_HEIGHT);
        assert.equal(result.cssWidth, CANVAS_WIDTH);
        assert.equal(result.cssHeight, CANVAS_HEIGHT);
        assert.equal(result.dpr, 1);
    });

    it('integer mode scales by floored ratio with minimum scale 1', () => {
        const doubled = computeDisplaySize('integer', 1600, 900, 1);

        assert.equal(doubled.scale, 2);
        assert.equal(doubled.deviceWidth, CANVAS_WIDTH * 2);
        assert.equal(doubled.deviceHeight, CANVAS_HEIGHT * 2);

        const single = computeDisplaySize('integer', 900, 500, 1);

        assert.equal(single.scale, 1);
        assert.equal(single.deviceWidth, CANVAS_WIDTH);
        assert.equal(single.deviceHeight, CANVAS_HEIGHT);
    });

    it('fit mode scales proportionally and keeps device dimensions at least 1', () => {
        const result = computeDisplaySize('fit', 400, 225, 1);

        assert.equal(result.scale, 0.5);
        assert.equal(result.deviceWidth, 400);
        assert.equal(result.deviceHeight, 225);
    });

    it('fit mode falls back to scale 1 for zero or negative viewport', () => {
        const zero = computeDisplaySize('fit', 0, 0, 1);

        assert.equal(zero.scale, 1);
        assert.equal(zero.deviceWidth, CANVAS_WIDTH);
        assert.equal(zero.deviceHeight, CANVAS_HEIGHT);

        const negative = computeDisplaySize('fit', -100, -50, 1);

        assert.equal(negative.scale, 1);
        assert.equal(negative.deviceWidth, CANVAS_WIDTH);
        assert.equal(negative.deviceHeight, CANVAS_HEIGHT);
    });

    it('applies dpr to viewport before scaling and converts css size back', () => {
        const result = computeDisplaySize('native', 400, 225, 2);

        assert.equal(result.dpr, 2);
        assert.equal(result.deviceWidth, CANVAS_WIDTH);
        assert.equal(result.deviceHeight, CANVAS_HEIGHT);
        assert.equal(result.cssWidth, CANVAS_WIDTH / 2);
        assert.equal(result.cssHeight, CANVAS_HEIGHT / 2);
    });

    it('treats invalid dpr as 1', () => {
        assert.equal(computeDisplaySize('native', 800, 450, 0).dpr, 1);
        assert.equal(computeDisplaySize('native', 800, 450, NaN).dpr, 1);
        assert.equal(computeDisplaySize('native', 800, 450, Infinity).dpr, 1);
    });
});
