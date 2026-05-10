window.GameNotesTracker = {
    dotNetRef: null,
    register(dotNetRef) {
        this.dotNetRef = dotNetRef;
    },
    unregister() {
        this.dotNetRef = null;
    },
    notify(eventType, from = "--", to = "--", isCapture = false) {
        if (!this.dotNetRef) return;
        this.dotNetRef.invokeMethodAsync("OnBoardEvent", eventType, from, to, isCapture);
    }
};

function toNotation(position) {
    if (!position || position.verticalOrientation < 0 || position.verticalOrientation > 7 || position.horizontalOrientation < 0 || position.horizontalOrientation > 7) {
        return "--";
    }

    const file = String.fromCharCode("a".charCodeAt(0) + position.horizontalOrientation);
    const rank = 8 - position.verticalOrientation;
    return `${file}${rank}`;
}

window.BuildBoard = {
    Build(JsonBlocks, figureColor, dotNetRef) {
        const blocks = JSON.parse(JsonBlocks);
        const board = document.getElementById("chessboard");
        if (!board) return;

        board.innerHTML = "";

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
    width: 100%;
    height: 100%;
    box-sizing: border-box;
    border: 0;
    transition: background-color 0.5s cubic-bezier(0.25, 1, 0.5, 1), transform .2s;
    background-color: ${block.HighlightColor ?? (block.BlockColor === 0 ? "gray" : "white")};
`;

    cell.addEventListener("click", () => dotNetRef.invokeMethodAsync("OnCellClick", i, j));

    const realColor = cell.style.backgroundColor;
    cell.addEventListener("mouseenter", () => {
        const baseColor = cell.style.backgroundColor;
        if (baseColor === "rgba(0, 255, 0, 0.45)") return;

        if (baseColor === "white" || baseColor === "gray") {
            cell.style.backgroundColor = "rgb(240, 216, 107)";
            cell.style.transform = "scale(1.05)";
        }
    });

    cell.addEventListener("mouseleave", () => {
        const baseColor = cell.style.backgroundColor;
        if (baseColor === "rgba(0, 255, 0, 0.45)") return;

        if (baseColor === "rgb(240, 216, 107)") {
            cell.style.backgroundColor = realColor;
            cell.style.transform = "scale(1)";
        }
    });

    if (block.Figure) {
        cell.appendChild(createPiece(block.Figure));
    }

    return cell;
}

function createPiece(figure) {
    const piece = document.createElement("img");
    const colorFolder = figure.FigureColor === 1 ? "black" : "white";
    const type = figure.$type.split(".").pop();

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
        if(castlingInfosDTOs) {
            castlingInfosDTOs.forEach(castling => {
                if (castling.isCastling) {
                    const vertical = castling.castlingPosition.verticalOrientation;
                    const horizontal = castling.castlingPosition.horizontalOrientation;
                    const cell = document.getElementById(`${vertical}${horizontal}`);
                    if (!cell) return;
                    cell.className = "castable";
                    highlightCell(cell, "rgba(148, 199, 145, 0.45)");
                }
            });
        }
        if(cutableBlocks) {
            cutableBlocks.forEach(block => {
                const vertical = block.position.verticalOrientation;
                const horizontal = block.position.horizontalOrientation;
                const cell = document.getElementById(`${vertical}${horizontal}`);
                if (!cell) return;
                cell.className = "cutable";
                highlightCell(cell, "rgba(255, 0, 0, 0.45)");
            });
        }
        if(movableBlocks) {
            movableBlocks.forEach(block => {
                const vertical = block.position.verticalOrientation;
                const horizontal = block.position.horizontalOrientation;
                const cell = document.getElementById(`${vertical}${horizontal}`);
                if (!cell) return;
                cell.className = "movable";
                highlightCell(cell, "rgba(0, 255, 0, 0.45)");
            });
        }
    },
    Clear: function (figureColor) {
        const allCells = document.querySelectorAll("[class^='cutable'], [class^='movable'], [class^='castable']");

        allCells.forEach(cell => {
            const id = cell.id;
            const indexI = parseInt(id[0], 10);
            const indexJ = parseInt(id[1], 10);
            cell.className = "";
            cell.style.transition = "background-color 0.2s ease, border-radius 0.2s ease";

            let backgroundColor;
            if (figureColor === 1) {
                backgroundColor = (indexI + indexJ) % 2 === 1 ? "white" : "gray";
            } else {
                backgroundColor = (14 - indexI + indexJ) % 2 === 1 ? "white" : "gray";
            }

            cell.style.backgroundColor = backgroundColor;
            cell.style.borderRadius = "0";
        });
    }
};

window.UpdateBoardAfterMove = {
    Move: function (from, to) {
        if (!from || !to) return;

        const fromCell = document.getElementById(`${from.verticalOrientation}${from.horizontalOrientation}`);
        const toCell = document.getElementById(`${to.verticalOrientation}${to.horizontalOrientation}`);

        if (fromCell && toCell) {
            const piece = fromCell.querySelector("img");
            const pieceTo = toCell.querySelector("img");
            if (piece && !pieceTo) {
                fromCell.removeChild(piece);
                toCell.appendChild(piece);
                ShowMovableAndCutableBlocks.Clear();
                GameNotesTracker.notify("move", toNotation(from), toNotation(to), false);
            }
        }
    }
};

window.UpdateBoardAfterCut = {
    Cut: function (from, to) {
        if (!from || !to) return;

        const fromCell = document.getElementById(`${from.verticalOrientation}${from.horizontalOrientation}`);
        const toCell = document.getElementById(`${to.verticalOrientation}${to.horizontalOrientation}`);

        if (fromCell && toCell) {
            const pieceFrom = fromCell.querySelector("img");
            const pieceTo = toCell.querySelector("img");
            if (pieceFrom && pieceTo) {
                fromCell.removeChild(pieceFrom);
                toCell.removeChild(pieceTo);
                toCell.appendChild(pieceFrom);
                ShowMovableAndCutableBlocks.Clear();
                GameNotesTracker.notify("capture", toNotation(from), toNotation(to), true);
            }
        }
    }
};

window.KingCheckedNotification = {
    Notify: function (kingPosition) {
        const cell = document.getElementById(`${kingPosition.verticalOrientation}${kingPosition.horizontalOrientation}`);
        if (!cell) return;

        cell.classList.add("king-blink-flash");
        GameNotesTracker.notify("check");

        cell.addEventListener("animationend", () => {
            cell.classList.remove("king-blink-flash");
        }, { once: true });
    }
};

window.KingMateNotification = {
    Notify: function (kingPosition, player, win) {
        const overlay = document.getElementById("mate-modal");
        const title = document.getElementById("mate-title");
        const info = document.getElementById("mate-info");

        if (title) {
            title.innerText = win ? "🎉 YOU WIN!" : "❌ YOU LOST!";
            title.style.color = win ? "green" : "red";
        }

        if (info) {
            info.innerText = `King position: ${kingPosition.verticalOrientation}${kingPosition.horizontalOrientation}`;
        }

        if (overlay) {
            overlay.classList.remove("hidden");
        }

        GameNotesTracker.notify("mate");

        const cellId = `${kingPosition.verticalOrientation}${kingPosition.horizontalOrientation}`;
        const cell = document.getElementById(cellId);
        if (cell) {
            cell.classList.add("king-blink-flash");
            cell.addEventListener("animationend", () => {
                cell.classList.remove("king-blink-flash");
            }, { once: true });
        }
    }
};

window.ReceiveBlockChangesHistory = {
    Change: function (changedBlocks) {
        changedBlocks.forEach(block => {
            if (!block || !block.position) return;

            const vertical = block.position.verticalOrientation;
            const horizontal = block.position.horizontalOrientation;
            const cell = document.getElementById(`${vertical}${horizontal}`);
            if (!cell) return;

            const oldPiece = cell.querySelector("img");
            if (oldPiece) cell.removeChild(oldPiece);

            if (block.figure) {
                const img = document.createElement("img");
                const colorFolder = block.figure.figureColor === 1 ? "black" : "white";
                const figureType = block.figure.$type.split(".").pop();
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

            GameNotesTracker.notify("helper", toNotation(from), toNotation(to), false);
        }
    }
};
