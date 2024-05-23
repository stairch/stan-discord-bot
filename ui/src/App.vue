<script setup lang="ts">
import { RouterLink, RouterView } from "vue-router";
</script>

<template>
    <header>
        <img
            alt="Vue logo"
            class="logo"
            src="/logo_protected_inverted.png"
            height="100"
        />
        <nav>
            <RouterLink to="/">Home</RouterLink>
            <RouterLink to="/announcement">Announcements</RouterLink>
        </nav>
    </header>

    <div id="container">
        <RouterView />
    </div>
</template>

<style scoped>
header {
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-right: 2rem;

    position: sticky;
    top: 0;
}

nav {
    display: flex;
    gap: 1rem;
    font-size: 1rem;

    & a {
        color: var(--c-white-1);

        &:hover {
            color: var(--c-stair-graphite);
        }

        &.router-link-active {
            text-decoration: underline;
        }
    }
}

#container {
    max-width: 1280px;
    margin: 0 auto;
    padding: 2rem;
}
</style>
