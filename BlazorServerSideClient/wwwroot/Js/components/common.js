window.getCookie = function (name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(";").shift();
    return null;
};

window.ErrorModal = {
    dotNetRef: null,

    Register: function (dotNetHelper) {
        this.dotNetRef = dotNetHelper;
    },

    Show: function (message) {
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync("ShowFromJs", message);
        }
    },

    Hide: function () {
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync("HideFromJs");
        }
    }
};

window.inviteModal = {
    intervalId: null,

    hide: function () {
        const el = document.getElementById("invite-backdrop-Id");
        if (el) el.hidden = true;

        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
    },

    show: function (time, name) {
        const invitationBasePage = document.getElementById("invite-backdrop-Id");
        if (invitationBasePage) invitationBasePage.hidden = false;

        const timeValue = document.getElementById("Inviter-Time-Value");
        const nameValue = document.getElementById("Inviter-Name");

        if (!timeValue || !nameValue) return;

        timeValue.innerText = String(time);
        nameValue.innerText = name;

        if (this.intervalId) {
            clearInterval(this.intervalId);
        }

        this.intervalId = setInterval(() => {
            let current = parseInt(timeValue.innerText, 10);

            if (current > 0) {
                timeValue.innerText = String(current - 1);
            } else {
                clearInterval(this.intervalId);
                this.intervalId = null;
                this.hide();
            }
        }, 4000);
    }
};

window.Players = {
    show: function (gamer1, gamer2) {
        const player1Element = document.getElementById("player1Name");
        const player2Element = document.getElementById("player1Meta");

        if (player1Element) player1Element.innerHTML = "Player1: " + gamer1;
        if (player2Element) player2Element.innerHTML = "Player2: " + gamer2;
    }
};

window.NavigateTo = function (path) {
    window.location.href = path;
};

window.GameDiv = {
    Disable: function (gameClassName) {
        const elements = document.getElementsByClassName(gameClassName);

        for (let i = 0; i < elements.length; i++) {
            const parent = elements[i];
            parent.style.position = "relative";

            const overlay = document.createElement("div");
            overlay.className = "game-overlay";
            overlay.innerHTML = `
                <div class="loader-container">
                    <div class="spinner"></div>
                    <div class="loading-text">Calculating best move...</div>
                </div>
            `;

            parent.appendChild(overlay);
        }
    },

    Enable: function (gameClassName) {
        const elements = document.getElementsByClassName(gameClassName);

        for (let i = 0; i < elements.length; i++) {
            const overlay = elements[i].querySelector(".game-overlay");
            if (overlay) overlay.remove();
        }
    }
};

window.OpponentDisconnected = {
    Notify: function (message) {
        const notification = document.createElement("div");
        notification.innerText = message;

        notification.style.position = "fixed";
        notification.style.top = "20px";
        notification.style.right = "20px";
        notification.style.padding = "15px 25px";
        notification.style.backgroundColor = "#2ecc71";
        notification.style.color = "white";
        notification.style.fontSize = "16px";
        notification.style.borderRadius = "8px";
        notification.style.boxShadow = "0 4px 12px rgba(0,0,0,0.2)";
        notification.style.zIndex = "9999";

        document.body.appendChild(notification);

        setTimeout(() => {
            notification.remove();
            window.location.href = "/dashboard";
        }, 1000);
    }
};
