<script setup lang="ts">
import { RouterLink, RouterView } from "vue-router";
import "@discord-message-components/vue/styles";
import { ref, onMounted } from "vue";
import Forbidden from "./views/Forbidden.vue";

const userData = ref<any | null>(null);
const showUserOptions = ref(false);

onMounted(async () => {
    const res = await fetch("/api/auth/me")
    if (res.ok) {
        userData.value = await res.json();
    } else {
        userData.value = false;
    }

    document.body.addEventListener("click", () => {
        showUserOptions.value = false;
    });
});
</script>

<template>
    <Forbidden v-if="userData == false" />
    <div v-else-if="userData">
        <header>
            <router-link to="/">
                <img
                    alt="Stair Logo"
                    class="logo"
                    src="/logo_protected_inverted.png"
                    height="50"
                />
            </router-link>
            <nav>
                <RouterLink title="Manage Discord Users" to="/discord/users"><span class="material-symbols-rounded">group</span></RouterLink>
                <RouterLink title="Announcements" to="/announcements"><span class="material-symbols-rounded">campaign</span></RouterLink>

                <div @click.stop="showUserOptions = true" class="user">
                    <span>{{userData.displayName}}</span>
                    <div @click.stop class="options" v-if="showUserOptions">
                        <div class="action">
                            <a href='/api/auth/signout'>
                                Sign out
                                <span class="material-symbols-rounded">logout</span>
                            </a>
                        </div>
                        <div class="data">
                            <h2>{{ userData.userPrincipalName }}</h2>
                            <p>{{ userData.mail }}</p>
                        </div>
                    </div>
                </div>
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

    .user {
        cursor: pointer;
        height: 100%;
        position: relative;
        display: flex;
        align-items: center;
        padding: 0.5rem;

        &:hover {
            background: rgba(255, 255, 255, 0.1);
        }

        .options {
            position: absolute;
            cursor: auto;
            top: 100%;
            right: 0;
            background: var(--bg-soft);
            border: 1px solid var(--bg-muted);
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
            border-radius: 0.5rem;
            padding: 1rem;
            display: flex;
            flex-direction: column;
            gap: 2rem;
            color: var(--fg-text);

            .action {
                display: flex;
                justify-content: flex-end;
            }

            a {
                color: var(--fg-text);
                display: block;
                padding: 0.5rem;
                text-align: center;
                text-decoration: none;

                &:hover {
                    text-decoration: underline;
                }

                & span {
                    font-size: 1rem;
                    margin-left: 0.5em;
                    translate: 0 0.1em;
                }
            }

            .data {
                padding: 0 1rem;
            }        
        }
    }
}

#container {
    max-width: 1280px;
    margin: 0 auto;
    padding: 2rem;
}
</style>
