const path = require("path");
module.exports = {
    utils: {
        resolve: function(dir) {
            return path.join(__dirname, "..", dir);
        },
        assetsPath: function(_path) {
            const assetsSubDirectory = "static";
            return path.posix.join(assetsSubDirectory, _path);
        }
    }
};
