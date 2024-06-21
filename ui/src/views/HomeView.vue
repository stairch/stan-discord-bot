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
                <router-link to="/discord/users">
                    <button>
                        <span class="material-symbols-rounded">explore</span>
                        Explore
                    </button>
                </router-link>
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

    @media (max-width: 768px) {
        flex-direction: column-reverse;
        align-items: flex-start;
    }

    &:nth-child(even) {
        flex-direction: row-reverse;

        .info {
            align-items: flex-end;
        }

        @media (max-width: 768px) {
            flex-direction: column-reverse;
            
            .info {
                align-items: flex-start;
            }
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
</style>
