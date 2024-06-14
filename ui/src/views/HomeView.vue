<template>
    <main>
        <div class=section>
            <div class="info">
                <h1>Announcements</h1>
                <router-link to="/announcements">
                    <button>
                        <span class="material-symbols-rounded">add</span>
                        Create Announcement
                    </button>
                </router-link>
            </div>
            <div class="icon">
                <span class="material-symbols-rounded">campaign</span>
            </div>
        </div>
        <div class=section>
            <div class="info">
                <h1>Discord User Management</h1>
                <p>Soon</p>
            </div>
            <div class="icon">
                <span class="material-symbols-rounded">group</span>
            </div>
        </div>
    </main>
</template>

<style scoped>
.section {
    min-height: 40vh;
    display: flex;
    flex-direction: row;
    align-items: end;
    position: relative;

    .info {
        flex: 1;
        display: flex;
        flex-direction: column;
        align-items: start;
    }

    &:nth-child(even) {
        flex-direction: row-reverse;

        .info {
            align-items: flex-end;
        }
    }

    .icon span {
        font-size: 10rem;
    }

    &:not(:last-child) {
        border-bottom: 1px solid var(--bg-muted);
        padding-bottom: 2em;
        margin-bottom: 2em;
    }
}

a button {
    display: flex;
    align-items: center;
    gap: 1em;
}
</style>
