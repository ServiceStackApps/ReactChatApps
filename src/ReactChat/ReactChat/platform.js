var WebNativeHost = (function () {
    function WebNativeHost() {
        this.platform = "web";
    }
    WebNativeHost.prototype.showAbout = function () {
        alert("DefaultApp - ServiceStack + React");
    };
    WebNativeHost.prototype.toggleFormBorder = function () { };
    WebNativeHost.prototype.quit = function () {
        window.close();
    };
    WebNativeHost.prototype.ready = function () { };
    return WebNativeHost;
}());
window.nativeHost = window.nativeHost || new WebNativeHost();
//# sourceMappingURL=platform.js.map