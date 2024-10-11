export const throttle = (callback: Function, delay: number) => {
    var wait = false;
    return function () {
        if (!wait) {
            callback();
            wait = true;
            setTimeout(function () {
                wait = false;
            }, delay);
        }
    };
};

export const debounce = (callback: Function, delay: number) => {
    var timeout: number;
    return function () {
        clearTimeout(timeout);
        timeout = setTimeout(() => {
            callback();
        }, delay);
    };
};
