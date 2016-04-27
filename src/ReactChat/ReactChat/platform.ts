interface Window {
    nativeHost: any;
}

class WebNativeHost {

    platform = "web";

    showAbout() {
        alert("DefaultApp - ServiceStack + React");
    }

    toggleFormBorder() { }

    quit() {
        window.close();
    }

    ready() { }
}

window.nativeHost = window.nativeHost || new WebNativeHost();