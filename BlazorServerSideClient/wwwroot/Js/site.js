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
        var player1Element = document.getElementById("player1Name");
        var player2Element = document.getElementById("player1Meta");

        player1Element.innerHTML = "Player1: " + gamer1;
        player2Element.innerHTML = "Player2: " + gamer2;
    }
}

window.BuildBoard = {
    Build: function (JsonBlocks, figureColor, dotNetRef) {
        const blocks = JSON.parse(JsonBlocks);
        const mainBoardDiv = document.getElementById("chessboard");
        mainBoardDiv.innerHTML = "";
        mainBoardDiv.style.display = "grid";
        mainBoardDiv.style.gridTemplateRows = "repeat(8, 1fr)";
        mainBoardDiv.style.gridTemplateColumns = "repeat(8, 1fr)";
        mainBoardDiv.style.width = "800px";
        mainBoardDiv.style.height = "800px";
        mainBoardDiv.style.gap = "2px";

        var startIndex = figureColor === 1 ? 7 : 0;
        var icrement = figureColor === 1 ? -1 : +1;

        var j = startIndex;
        var i = startIndex;

        while (i >= 0 && i < 8) {
            j = startIndex;
            while (j >= 0 && j < 8) {
                const block = blocks[i][j];

                const cell = document.createElement("div");
                cell.style.position = "relative";
                cell.style.width = "100px";
                cell.style.height = "100px";
                cell.style.boxSizing = "border-box";
                cell.style.border = "1px solid #000";
                cell.style.transition = "background-color 0.5s cubic-bezier(0.25, 1, 0.5, 1)";
                cell.style.backgroundColor = block.BlockColor === 0 ? "gray" : "white";

                cell.addEventListener("click", () => {
                    const loggerDiv = document.getElementById("Logger-Div");
                    if (loggerDiv.children.length > 10)
                        loggerDiv.innerHTML = "";
                    const loggerInfo = document.createElement("div");
                    loggerInfo.classList.add("log-item");

                    loggerInfo.innerHTML =
                        `<span class="log-pos">Cell: (${i},${j})</span>
                         <span class="log-figure">${block.Figure?.$type ?? "Empty"}</span>`;

                    loggerDiv.appendChild(loggerInfo);
                    const id = cell.id;

                    const indexI = parseInt(id[0]);
                    const indexJ = parseInt(id[1]);

                    dotNetRef.invokeMethodAsync("OnCellClick", indexI, indexJ);
                });
                if (block.HighlightColor) {
                    cell.style.backgroundColor = block.HighlightColor;
                }
                cell.id = `${i}${j}`;
                if (block.Figure) {
                    const piece = document.createElement("img");
                    piece.style.position = "absolute";
                    piece.style.top = "50%";
                    piece.style.left = "50%";
                    piece.style.transform = "translate(-50%, -50%)";
                    piece.style.width = "80%";
                    piece.style.height = "80%";
                    piece.style.zIndex = "10";

                    var colorFolder;
                    if (block.Figure.FigureColor === 1) {
                        colorFolder = "black";
                    } else {
                        colorFolder = "white";
                    }

                    const figureType = block.Figure.$type.split('.').pop();
                    piece.src = `/PNGs/${colorFolder}/${figureType}.png`;
                    cell.appendChild(piece);
                }
                const originalColor = cell.style.backgroundColor;
                cell.addEventListener("mouseenter", () => {
                    if (cell.style.backgroundColor != "white" && cell.style.backgroundColor != "gray")
                        return;

                    cell.style.backgroundColor = "rgb(240, 216, 107)";
                    cell.style.transform = "scale(1.05)";
                });
                cell.addEventListener("mouseleave", () => {
                    if (cell.style.backgroundColor != "rgb(240, 216, 107)")
                        return;

                    cell.style.backgroundColor = originalColor;
                    cell.style.transform = "scale(1)";
                });

                mainBoardDiv.appendChild(cell);
                j += icrement;
            }
            i += icrement;
        }
    }
};
window.ShowMovableAndCutableBlocks = {
    Paint: function (cutableBlocks, movableBlocks) {

        function highlightCell(cell, color) {

            if (!cell) return;

            cell.style.transition = "background-color 0.3s ease, transform 0.3s ease, border-radius 0.3s ease";

            cell.style.backgroundColor = color;

            cell.style.transform = "scale(1.12)";
            cell.style.borderRadius = "8px";

            setTimeout(() => {
                cell.style.transform = "scale(1)";
            }, 300);
        }

        cutableBlocks.forEach(block => {
            const vertical = block.position.verticalOrientation;
            const horizontal = block.position.horizontalOrientation;
            const cell = document.getElementById(`${vertical}${horizontal}`);
            cell.className = "cutable";
            highlightCell(cell, "rgba(255, 0, 0, 0.45)");
        });

        movableBlocks.forEach(block => {
            const vertical = block.position.verticalOrientation;
            const horizontal = block.position.horizontalOrientation;
            const cell = document.getElementById(`${vertical}${horizontal}`);
            cell.className = "movable";
            highlightCell(cell, "rgba(0, 255, 0, 0.45)");
        });
    },
    Clear: function (figureColor) {

        const allCellsCutable = document.querySelectorAll("[class^='cutable']");
        const allCellsMovable = document.querySelectorAll("[class^='movable']");
        if (allCellsCutable)
            allCellsCutable.forEach(cell => {
                const id = cell.id;
                const indexI = parseInt(id[0]);
                const indexJ = parseInt(id[1]);
                cell.className = "";
                cell.style.transition = "background-color 0.2s ease, border-radius 0.2s ease";

                var backgroundColor
                if (figureColor == 1)
                    backgroundColor = (indexI + indexJ) % 2 == 1 ? "white" : "gray";
                else {
                    backgroundColor = (14 - (indexI) + indexJ) % 2 == 1 ? "white" : "gray";
                }

                cell.style.backgroundColor = backgroundColor;
                cell.style.borderRadius = "0px";
            });
        if (allCellsMovable)
            allCellsMovable.forEach(cell => {
                const id = cell.id;
                const indexI = parseInt(id[0]);
                const indexJ = parseInt(id[1]);

                var backgroundColor
                if (figureColor == 1)
                    backgroundColor = (indexI + indexJ) % 2 == 1 ? "white" : "gray";
                else {
                    backgroundColor = (14 - (indexI) + indexJ) % 2 == 1 ? "white" : "gray";
                }

                cell.className = "";
                cell.style.transition = "background-color 0.2s ease, border-radius 0.2s ease";
                cell.style.backgroundColor = backgroundColor;
                cell.style.borderRadius = "0px";
            });

    }
};

window.UpdateBoardAfterMove = {
    Move: function (from, to, myColor) {

        if (!from || !to)
            return;
        var fromCell = document.getElementById(`${from.verticalOrientation}${from.horizontalOrientation}`);
        var toCell = document.getElementById(`${to.verticalOrientation}${to.horizontalOrientation}`);

        if (fromCell && toCell) {
            var piece = fromCell.querySelector("img");
            var pieceTo = toCell.querySelector("img");
            if (piece && !pieceTo) {
                fromCell.removeChild(piece);
                toCell.appendChild(piece);
                ShowMovableAndCutableBlocks.Clear();
            }
        }
    },
}

window.UpdateBoardAfterCut = {
    Cut: function (from, to, myColor) {
        if (!from || !to)
            return;
        var fromCell = document.getElementById(`${from.verticalOrientation}${from.horizontalOrientation}`);
        var toCell = document.getElementById(`${to.verticalOrientation}${to.horizontalOrientation}`);

        if (fromCell && toCell) {
            var pieceFrom = fromCell.querySelector("img");
            var pieceTo = toCell.querySelector("img");
            if (pieceFrom && pieceTo) {
                fromCell.removeChild(pieceFrom);
                toCell.removeChild(pieceTo);
                toCell.appendChild(pieceFrom);
                ShowMovableAndCutableBlocks.Clear();
            }
        }
    }
}
window.KingCheckedNotification = {
    Notify: function (kingPosition) {
        const cell = document.getElementById(`${kingPosition.verticalOrientation}${kingPosition.horizontalOrientation}`);
        if (!cell) return;

        cell.classList.add("king-blink-flash");

        cell.addEventListener("animationend", () => {
            cell.classList.remove("king-blink-flash");
        }, { once: true });
    }
};
