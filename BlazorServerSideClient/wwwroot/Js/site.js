window.getCookie = function (name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
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
        }, 4000);
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
    Build(JsonBlocks, figureColor, dotNetRef) {
        const blocks = JSON.parse(JsonBlocks);
        const board = document.getElementById("chessboard");

        // Reset board
        board.innerHTML = "";
        board.style.cssText = `
            display: grid;
            grid-template: repeat(8, 1fr) / repeat(8, 1fr);
            width: 900px;
            height: 900px;
            gap: -100px;
        `;

        const start = figureColor === 1 ? 7 : 0;
        const step = figureColor === 1 ? -1 : 1;

        const fragment = document.createDocumentFragment();

        for (let i = start; i >= 0 && i < 8; i += step) {
            for (let j = start; j >= 0 && j < 8; j += step) {
                const block = blocks[i][j];
                const cell = createCell(i, j, block, dotNetRef);
                fragment.appendChild(cell);
            }
        }

        board.appendChild(fragment);
    }
};

function createCell(i, j, block, dotNetRef) {
    const cell = document.createElement("div");
    cell.id = `${i}${j}`;
    cell.style.cssText = `
        position: relative;
        width: 100px;
        height: 100px;
        box-sizing: border-box;
        border: 0px solid #000;
        transition: background-color 0.5s cubic-bezier(0.25, 1, 0.5, 1), transform .2s;
        background-color: ${block.HighlightColor ?? (block.BlockColor === 0 ? "gray" : "white")};
    `;

    // Click
    cell.addEventListener("click", () =>
        dotNetRef.invokeMethodAsync("OnCellClick", i, j)
    );

    // Hover
    var realColor = cell.style.backgroundColor;
    cell.addEventListener("mouseenter", () => {
        const baseColor = cell.style.backgroundColor;

        if ((baseColor === "rgba(0, 255, 0, 0.45)") || (baseColor === "rgba(0, 255, 0, 0.45)")) return;
        if ((baseColor == "white" || baseColor == "gray")) {
            cell.style.backgroundColor = "rgb(240, 216, 107)";
            cell.style.transform = "scale(1.05)";
        }
    });
    cell.addEventListener("mouseleave", () => {
        const baseColor = cell.style.backgroundColor;

        if ((baseColor === "rgba(0, 255, 0, 0.45)") || (baseColor === "rgba(0, 255, 0, 0.45)")) return;

        if (baseColor !== "white" && baseColor !== "gray" && baseColor === "rgb(240, 216, 107)") {
            cell.style.backgroundColor = realColor;
            cell.style.transform = "scale(1)";
        }
    });

    // Figure
    if (block.Figure) {
        cell.appendChild(createPiece(block.Figure));
    }

    return cell;
}

function createPiece(figure) {
    const piece = document.createElement("img");
    const colorFolder = figure.FigureColor === 1 ? "black" : "white";
    const type = figure.$type.split('.').pop();

    piece.src = `/PNGs/${colorFolder}/${type}.png`;
    piece.style.cssText = `
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: 80%;
        height: 80%;
        z-index: 10;
    `;

    return piece;
}

window.NavigateTo = function (path) {
    window.location.href = path;
}
window.ShowMovableAndCutableBlocks = {
    Paint: function (cutableBlocks, movableBlocks, castlingInfosDTOs) {

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

        castlingInfosDTOs.forEach(castling => {
            if (castling.isCastling) {
                const vertical = castling.castlingPosition.verticalOrientation;
                const horizontal = castling.castlingPosition.horizontalOrientation;
                const cell = document.getElementById(`${vertical}${horizontal}`);
                cell.className = "castable";
                highlightCell(cell, "rgba(148, 199, 145, 0.45)");
            }
        });

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
        const allCellsCastable = document.querySelectorAll("[class^='castable']");
        if (allCellsCastable)
            allCellsCastable.forEach(cell => {
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
        }, {once: true});
    }
};

window.KingMateNotification = {
    Notify: function (kingPosition, player, win) {

        const overlay = document.getElementById("mate-modal");
        const title = document.getElementById("mate-title");
        const info = document.getElementById("mate-info");
        const btn = document.getElementById("mate-btn");

        title.innerText = win ? "🎉 YOU WIN!" : "❌ YOU LOST!";
        title.style.color = win ? "green" : "red";
        info.innerText = `King position: ${kingPosition.verticalOrientation}${kingPosition.horizontalOrientation}`;

        overlay.classList.remove("hidden");

        const cellId = `${kingPosition.verticalOrientation}${kingPosition.horizontalOrientation}`;
        const cell = document.getElementById(cellId);

        if (cell) {
            cell.classList.add("king-blink-flash");
            cell.addEventListener("animationend", () => {
                cell.classList.remove("king-blink-flash");
            }, {once: true});
        }
    }
};


document.addEventListener("DOMContentLoaded", () => {
    const prevBtn = document.getElementById("prevBtn");
    const nextBtn = document.getElementById("nextBtn");

    function pulse(btn) {
        btn.classList.add("pulse");
        setTimeout(() => btn.classList.remove("pulse"), 200);
    }

    if (prevBtn)
        prevBtn.addEventListener("click", () => {
            pulse(prevBtn);
        });
    if (nextBtn)
        nextBtn.addEventListener("click", () => {
            pulse(nextBtn);
        });
});


document.addEventListener('DOMContentLoaded', () => {
    const cards = document.querySelectorAll('.game-card');

    cards.forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = (e.clientX - rect.left) / rect.width - 0.5;
            const y = (e.clientY - rect.top) / rect.height - 0.5;
            const tx = x * 6;
            const ty = y * -6;
            card.style.transform = `translateY(-10px) rotate(${tx}deg)`;
            card.style.transition = 'transform 0.05s';
        });
        card.addEventListener('mouseleave', () => {
            card.style.transform = '';
            card.style.transition = 'transform .25s cubic-bezier(.2,.9,.3,1)';
        });
    });

    const smallNext = document.querySelectorAll('.small-next');
    smallNext.forEach(btn => {
        btn.addEventListener('mouseenter', () => btn.animate([{transform: 'scale(1)'}, {transform: 'scale(1.06)'}, {transform: 'scale(1)'}], {duration: 420}));
    });
});


window.ReceiveBlockChangesHistory = {
    Change: function (changedBlocks) {

        changedBlocks.forEach(block => {


            if (!block || !block.position) return;

            const vertical = block.position.verticalOrientation;
            const horizontal = block.position.horizontalOrientation;

            const cell = document.getElementById(`${vertical}${horizontal}`);
            if (!cell) return;


            const oldPiece = cell.querySelector("img");
            if (oldPiece) {
                cell.removeChild(oldPiece);
            }

            if (block.figure) {
                const img = document.createElement("img");
                var colorFolder;
                if (block.figure.figureColor === 1) {
                    colorFolder = "black";
                } else {
                    colorFolder = "white";
                }

                const figureType = block.figure.$type.split('.').pop();
                img.style.position = "absolute";
                img.style.top = "50%";
                img.style.left = "50%";
                img.style.transform = "translate(-50%, -50%)";
                img.style.width = "80%";
                img.style.height = "80%";
                img.style.zIndex = "10";
                img.src = `/PNGs/${colorFolder}/${figureType}.png`;
                img.draggable = false;
                cell.appendChild(img);
            }
        });

        ShowMovableAndCutableBlocks.Clear();
    }
};
window.ReceiveOptimalMoves = {
    Show: function (from, to) {
        const fromCell = document.getElementById(`${from.verticalOrientation}${from.horizontalOrientation}`);
        const toCell = document.getElementById(`${to.verticalOrientation}${to.horizontalOrientation}`);

        if (fromCell && toCell) {
            const originalFromBg = fromCell.style.backgroundColor;
            const originalToBg = toCell.style.backgroundColor;


            fromCell.style.backgroundColor = "#800000";
            toCell.style.backgroundColor = "#800000";

            setTimeout(() => {
                fromCell.style.backgroundColor = originalFromBg;
                toCell.style.backgroundColor = originalToBg;
            }, 4000);
        }
    }
};

window.GameDiv = {

    Disable: function (gameClassName) {
        var elements = document.getElementsByClassName(gameClassName);

        for (var i = 0; i < elements.length; i++) {

            var parent = elements[i];

            parent.style.position = "relative";

            var overlay = document.createElement("div");
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
        var elements = document.getElementsByClassName(gameClassName);

        for (var i = 0; i < elements.length; i++) {
            var overlay = elements[i].querySelector(".game-overlay");
            if (overlay)
                overlay.remove();
        }
    }

};

window.OpponentDisconnected = {
    Notify: function (message) {

        let notification = document.createElement("div");
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

