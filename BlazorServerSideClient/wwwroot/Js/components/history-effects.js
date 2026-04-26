document.addEventListener("DOMContentLoaded", () => {
    const prevBtn = document.getElementById("prevBtn");
    const nextBtn = document.getElementById("nextBtn");

    function pulse(btn) {
        btn.classList.add("pulse");
        setTimeout(() => btn.classList.remove("pulse"), 200);
    }

    if (prevBtn) {
        prevBtn.addEventListener("click", () => {
            pulse(prevBtn);
        });
    }

    if (nextBtn) {
        nextBtn.addEventListener("click", () => {
            pulse(nextBtn);
        });
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const cards = document.querySelectorAll(".game-card");

    cards.forEach(card => {
        card.addEventListener("mousemove", e => {
            const rect = card.getBoundingClientRect();
            const x = (e.clientX - rect.left) / rect.width - 0.5;
            const tx = x * 6;
            card.style.transform = `translateY(-10px) rotate(${tx}deg)`;
            card.style.transition = "transform 0.05s";
        });
        card.addEventListener("mouseleave", () => {
            card.style.transform = "";
            card.style.transition = "transform .25s cubic-bezier(.2,.9,.3,1)";
        });
    });

    const smallNext = document.querySelectorAll(".small-next");
    smallNext.forEach(btn => {
        btn.addEventListener("mouseenter", () =>
            btn.animate(
                [{ transform: "scale(1)" }, { transform: "scale(1.06)" }, { transform: "scale(1)" }],
                { duration: 420 }
            )
        );
    });
});
