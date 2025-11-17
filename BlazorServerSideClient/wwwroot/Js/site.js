window.getCookie = function (name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
};

window.inviteModal = {
    hide: function () {
        const el = document.getElementById("invite-backdrop-Id");
        if (el) el.hidden = true;
    },
    show: function () {
        const el = document.getElementById("invite-backdrop-Id");
        if (el) el.hidden = false;
    }
};
