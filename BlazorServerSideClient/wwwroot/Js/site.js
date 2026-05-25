// Intentionally minimal: logic moved into component scripts under /Js/components.

window.gameDisconnect = {
    register: function (connectionId) {
        function disconnect() {
            const data = new FormData();
            data.append("connectionId", connectionId);

            navigator.sendBeacon("/api/game/disconnect", data);
        }

        window.addEventListener("pagehide", disconnect);
        window.addEventListener("beforeunload", disconnect);
    }
};