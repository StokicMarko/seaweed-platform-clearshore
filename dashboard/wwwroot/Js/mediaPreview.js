// Each file input owns its temporary preview URL.
const previews = new Map();

export function createPreview(input) {
    releasePreview(input);

    const file = input.files[0];

    if (!file) {
        return "";
    }

    const url = URL.createObjectURL(file);

    previews.set(input, url);

    return url;
}

export function releasePreview(input) {
    const url = previews.get(input);

    if (url) {
        URL.revokeObjectURL(url);
        previews.delete(input);
    }
}

// Clean up when navigation removes the input.
const observer = new MutationObserver(() => {
    for (const input of previews.keys()) {
        if (!input.isConnected) {
            releasePreview(input);
        }
    }
});

observer.observe(document.documentElement, {
    childList: true,
    subtree: true
});