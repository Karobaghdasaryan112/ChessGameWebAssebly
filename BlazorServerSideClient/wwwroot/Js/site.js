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

window.Players = {

    show: function (gamer1, gamer2) {
        var player1Element = document.getElementById("Player1");
        var player2Element = document.getElementById("Player2");

        player1Element.innerHTML = "Player1: " + gamer1;
        player2Element.innerHTML = "Player2: " + gamer2;
    }
}
window.BuildBoard = {
    Build: function (board) {

        const mainBoardDiv = document.getElementById("chessboard");
        mainBoardDiv.innerHTML = ""; 

        const blocks = board.BoardBlocks;

        for (let i = 0; i < 8; i++) {

            const rowDiv = document.createElement("div");
            rowDiv.classList.add("row");
            mainBoardDiv.appendChild(rowDiv);

            for (let j = 0; j < 8; j++) {

                const block = blocks[i][j];

                const cell = document.createElement("div");
                cell.classList.add("cell");

                if (block.BlockColor === 0) {
                    cell.classList.add("light");
                } else {
                    cell.classList.add("dark");
                }

                if (block.Figure) {
                    const piece = document.createElement("img");
                    piece.classList.add("piece");

                    piece.src = `/PNGs/${block.Figure.Color}/${block.Figure.Type}.png`;

                    cell.appendChild(piece);
                }
                rowDiv.appendChild(cell);
            }
        }
    }
};
