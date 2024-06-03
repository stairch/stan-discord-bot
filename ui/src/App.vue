<script setup lang="ts">
import { RouterLink, RouterView } from "vue-router";
import "@discord-message-components/vue/styles";
import { ref, onMounted } from "vue";
import Forbidden from "./views/Forbidden.vue";

const userData = ref<any | null>(null);

onMounted(async () => {
    const res = await fetch("/api/auth/me")
    if (res.ok) {
        userData.value = await res.json();
    } else {
        userData.value = false;
    }
});
</script>

<template>
    <Forbidden v-if="userData == false" />
    <div v-else-if="userData">
        <header>
            <img
                alt="Stair Logo"
                class="logo"
                src="/logo_protected_inverted.png"
                height="50"
            />
            <nav>
                <RouterLink title="Home" to="/"><span class="material-symbols-rounded">home</span></RouterLink>
                <RouterLink title="Announcements" to="/announcements"><span class="material-symbols-rounded">campaign</span></RouterLink>
                <a :href="userData ? '/api/auth/signout' : '/api/auth/signin'">
                    <span v-if="userData">{{userData.displayName}}</span>
                    <span v-else class="material-symbols-rounded">login</span>
                </a>
            </nav>
        </header>

        <div id="container">
            <RouterView />
        </div>
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
    z-index: 1000;
}

nav {
    display: flex;
    align-items: center;
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
