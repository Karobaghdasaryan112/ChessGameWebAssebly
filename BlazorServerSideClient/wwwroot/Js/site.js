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
    Build: function (JsonBlocks) {
        const blocks = JSON.parse(JsonBlocks);
        const mainBoardDiv = document.getElementById("chessboard");
        mainBoardDiv.innerHTML = "";
        mainBoardDiv.style.display = "grid";
        mainBoardDiv.style.gridTemplateRows = "repeat(8, 1fr)";
        mainBoardDiv.style.gridTemplateColumns = "repeat(8, 1fr)";
        mainBoardDiv.style.width = "800px";
        mainBoardDiv.style.height = "800px";
        mainBoardDiv.style.gap = "2px";
        for (let i = 0; i < 8; i++) {
            for (let j = 0; j < 8; j++) {
                const block = blocks[i][j];

                const cell = document.createElement("div");
                cell.style.position = "relative";
                cell.style.width = "100px";
                cell.style.height = "100px";
                cell.style.boxSizing = "border-box";
                cell.style.border = "1px solid #000";
                cell.style.transition = "background-color 0.5s cubic-bezier(0.25, 1, 0.5, 1)";
                cell.style.backgroundColor = block.BlockColor === 0 ? "white" : "gray";
                if (block.HighlightColor) {
                    cell.style.backgroundColor = block.HighlightColor;
                }

                if (block.Figure) {
                    const piece = document.createElement("img");
                    piece.style.position = "absolute";
                    piece.style.top = "50%";
                    piece.style.left = "50%";
                    piece.style.transform = "translate(-50%, -50%)";
                    piece.style.width = "80%";
                    piece.style.height = "80%";
                    piece.style.zIndex = "10";

                    const figureColor = block.Figure.FigureColor === 1 ? "black" : "white";
                    const figureType = block.Figure.$type.split('.').pop();
                    piece.src = `/PNGs/${figureColor}/${figureType}.png`;

                    cell.appendChild(piece);
                }
                const originalColor = cell.style.backgroundColor;
                cell.addEventListener("mouseenter", () => {
                    cell.style.backgroundColor = "#f0d86b";
                    cell.style.transform = "scale(1.05)";
                });
                cell.addEventListener("mouseleave", () => {
                    cell.style.backgroundColor = originalColor;
                    cell.style.transform = "scale(1)";
                });

                mainBoardDiv.appendChild(cell);
            }
        }
    }
};

