<script lang="ts" setup>
import { ref } from "vue";

const done = ref(false)

defineProps({
    title: String,
    description: String,
});
</script>
<template>
    <div class="step" @click="done = !done" :class="{done}">
        <div class=info>
            <h4>{{ title }}</h4>
            <p class="text-muted">{{ description }}</p>
        </div>
        <span class=material-symbols-rounded>
            {{ done ? "check_circle" : "radio_button_unchecked" }}
        </span>
    </div>
</template>

<style scoped>
.step {
    display: grid;
    grid-template-columns: 1fr 5ch;
    align-items: center;
    padding: 1em;
    gap: 1em;
    border-radius: 1em;
    background-color: var(--bg-soft);
    border: 1px solid var(--bg-muted);
    cursor: pointer;

    &.done {
        background-color: var(--c-stair-green-20);
        border-color: var(--c-stair-green);

        .info {
            text-decoration: line-through;
        }
        
        & span {
            color: var(--c-stair-green);
        }
    }
}

h4 {
    margin-bottom: 0.5em;
    font-size: 1rem;
    font-weight: bold;
}

.text-muted {
    color: var(--fg-text-muted);
}
</style>
