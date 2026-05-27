let dotNetReference = null;
let isTicking = false;

function onScroll() {
    if (!dotNetReference || isTicking) {
        return;
    }

    isTicking = true;
    window.requestAnimationFrame(async () => {
        const scrollPosition = window.innerHeight + window.scrollY;
        const threshold = document.documentElement.scrollHeight - 360;

        if (scrollPosition >= threshold) {
            await dotNetReference.invokeMethodAsync("LoadMoreOnScroll");
        }

        isTicking = false;
    });
}

export function registerScrollPagination(reference) {
    dotNetReference = reference;
    window.addEventListener("scroll", onScroll, { passive: true });
}

export function unregisterScrollPagination() {
    window.removeEventListener("scroll", onScroll);
    dotNetReference = null;
    isTicking = false;
}
