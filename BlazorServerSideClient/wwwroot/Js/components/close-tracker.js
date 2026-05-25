// wwwroot/js/browser-close.js

window.browserCloseHandler = {
    register: function (dotNetHelper) {
        let called = false;

        function onBrowserClose(event) {
            if (called) {
                return;
            }

            called = true;

            dotNetHelper.invokeMethodAsync("OnBrowserClosed");
        }

        window.addEventListener("pagehide", onBrowserClose);
        window.addEventListener("beforeunload", onBrowserClose);
    }
};