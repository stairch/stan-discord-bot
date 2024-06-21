<script setup lang="ts">
import { api, type IDegreeProgramme } from '@/api';
import { onMounted, ref } from 'vue';


const programmes = ref<IDegreeProgramme[]>([]);

onMounted(async () => {
    programmes.value = await api.db.getDegreeProgrammes();
});
</script>
<template>
    <h1>Degree Programmes</h1>
    <div class="programmes">
        <div class=programme>
            <span class=swatch></span>
            <p>ID</p>
            <span>Category</span>
            <span>Channel</span>
            <span>Role</span>
        </div>
        <div v-for="programme in programmes" :key="programme.id" class="programme">
            <span class="swatch" :style="{ '--colour': programme.colour}"></span>
            <p>{{ programme.id }}</p>
            <span>{{ programme.category }}</span>
            <span>{{ programme.channel }}</span>
            <span>{{ programme.role }}</span>
        </div>
    </div>
</template>
<style scoped>
.programmes {
    display: flex;
    flex-direction: column;
}

.programme {
    display: grid;
    grid-template-columns: 10px repeat(auto-fill, minmax(200px, 1fr));
    gap: 1rem;
    padding: 1em;

    &:not(:last-child) {
        border-bottom: 1px solid var(--bg-muted);
    }
}

.swatch {
    width: 10px;
    height: 10px;
    border-radius: 50%;
    background-color: var(--colour);
}
</style>
