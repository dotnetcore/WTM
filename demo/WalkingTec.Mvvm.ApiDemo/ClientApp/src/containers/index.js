export default {
    editer: function () { return import('./editer').then(function (x) { return x.default; }); },
    /**WTM**/
    test: function () { return import('./test').then(function (x) { return x.default; }); },
    test2: function () { return import('./test2').then(function (x) { return x.default; }); }
    /**WTM**/
};
//# sourceMappingURL=index.js.map