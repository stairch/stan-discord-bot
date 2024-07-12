<template>
    <div class="loader"></div>
</template>
<style scoped>
    .loader {
        width: 60px;
        aspect-ratio: 1;
        --c1: no-repeat linear-gradient(#21b378 0 0);
        --c2: no-repeat linear-gradient(#03946c 0 0);
        --c3: no-repeat linear-gradient(#0b6a5b 0 0);
        background: var(--c1) 0 0, var(--c1) 33% 33%, var(--c2) 66% 66%,
            var(--c3) 99% 99%;
        animation: l17 1.2s infinite alternate;
    }
    @keyframes l17 {
        0%,
        10% {
            background-size: 20% 100%;
        }
        50% {
            background-size: 20% 20%;
        }
        90%,
        100% {
            background-size: 100% 20%;
        }
    }
</style>
