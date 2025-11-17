window.getCookie = function (name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
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

        const InvitationBasePage = document.getElementById("invite-backdrop-Id");
        if (InvitationBasePage) InvitationBasePage.hidden = false;

        const timeValue = document.getElementById("Inviter-Time-Value");
        const nameValue = document.getElementById("Inviter-Name");

        timeValue.innerText = time;
        nameValue.innerText = name;

        if (this.intervalId) {
            clearInterval(this.intervalId);
        }

        this.intervalId = setInterval(() => {
            let current = parseInt(timeValue.innerText);

            if (current > 0) {
                timeValue.innerText = current - 1;
            } else {
                clearInterval(this.intervalId);
                this.intervalId = null;

                 this.hide();
            }
        }, 1000);
    }
};
