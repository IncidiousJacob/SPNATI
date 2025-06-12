if (!kyouko_toshinou) var kyouko_toshinou = (function (root) {
    const ID = "kyouko_toshinou"
    const LABEL = "Kyouko"

    const exports = {};

    root.Sentry.setTag(`${ID}-loaded`, true);
    root.Sentry.addBreadcrumb({
        category: ID,
        message: `Initializing ${ID}.js...`,
        level: 'info'
    });

    function assertEquals(a, b) {
        if (a !== b) {
            throw new Error(`[${LABEL}] Assertion failure: ${a} !== ${b}`);
        }
    }

    function reportException(prefix, e) {
        console.log(`[${LABEL}] Exception swallowed ${prefix}: `);

        Sentry.withScope(function (scope) {
            scope.setTag(`${ID}-error`, true);
            scope.setExtra("where", prefix);
            captureError(e);
        });

        captureError(e);
    }
    exports.reportException = reportException;

    const registerFunction = function(name, fn) {
        exports[name] = function() {
            try {
                return fn.apply(null, arguments);
            } catch (e) {
                reportException(`in ${name}`)
            }
        }
    }

    registerFunction('setAltSummerStripOrder', (slot) => {
        const kyouko = players[slot];
        const tank = kyouko.clothing[1];
        const shorts = kyouko.clothing[3];
        assertEquals(tank.name, "tank top");
        assertEquals(shorts.name, "shorts");

        kyouko.clothing[1] = shorts;
        kyouko.clothing[3] = tank;
    });

    return exports;
}(this));