<script setup lang="ts">
    import { ref } from "vue";
    import Modal from "@/components/Modal.vue";

    const showModal = ref(false);

    const date = new Date();

    const open = () => {
        showModal.value = true;
    };

    defineExpose({
        open,
    });
</script>

<template>
    <Modal v-model="showModal">
        <div class="content">
            <h2>Shortcuts</h2>
            <div class="shortcut">
                <span>Save</span>
                <span class="keys"> <kbd>Ctrl</kbd> + <kbd>S</kbd> </span>
            </div>
            <div class="shortcut">
                <span>Bold</span>
                <span class="keys"> <kbd>Ctrl</kbd> + <kbd>B</kbd> </span>
            </div>
            <div class="shortcut">
                <span>Italic</span>
                <span class="keys"> <kbd>Ctrl</kbd> + <kbd>I</kbd> </span>
            </div>
            <div class="shortcut">
                <span>Hyperlink</span>
                <span class="keys"> <kbd>Ctrl</kbd> + <kbd>K</kbd> </span>
            </div>
            <h2>Working in this tool</h2>
            <p>
                Every discord post you make using this tool will automatically
                tag the role assigned to the channel you're posting in. This
                means that you <strong>do not</strong> need to tag the role in
                the post itself.
                <br />
                <br />
                You can use normal markdown formatting in your posts, or use the
                shortcuts listed above. The formatting
                <em>should</em> automatically translate to your target platform.
                Instagram formatting is limited so it is recommended to manually
                check the generated content.
            </p>
            <h2>Discord mentions & channels</h2>
            <div class="shortcut">
                <span>Mention a user</span>
                <span class="keys"> <kbd><@{user}></kbd> </span>
            </div>
            <div class="shortcut">
                <span>Mention a role</span>
                <span class="keys"> <kbd><@&{role}></kbd> </span>
            </div>
            <div class="shortcut">
                <span>Link to a channel</span>
                <span class="keys"> <kbd><#{channel}></kbd> </span>
            </div>
            <div class="shortcut">
                <span>Link "Channels & Roles"</span>
                <span class="keys"> <kbd>&lt;id:customize></kbd> </span>
            </div>
            <h2>Post guidelines</h2>
            <p>
                Guidelines for posts are outlined in the STAIR wiki.
                <br />
                <br />
                Important things to remember:
                <ul>
                    <li>Keep posts short and to the point</li>
                    <li>Use emojis to make posts more engaging 🎉</li>
                    <li>Highlight <strong>important information</strong> with bold text</li>
                    <li>
                        Format your dates and times consistently:
                        <ul>
                            <li>English: <kbd>{{ date.toLocaleDateString("en-GB", {dateStyle: "long"}) }}, {{ date.toLocaleTimeString("en-GB", { timeStyle: "short" }) }}</kbd></li>
                            <li>German: <kbd>{{ date.toLocaleDateString("de-DE", {dateStyle: "long"}) }}, {{ date.toLocaleTimeString("de-DE", { timeStyle: "short" }) }}</kbd></li>
                        </ul>
                    </li>
                </ul>
            </p>
        </div>
    </Modal>
</template>

<style scoped>
    .content {
        display: flex;
        flex-direction: column;
        gap: 1em;
        max-height: calc(80vh - 2em);
        padding-bottom: 1em;
        overflow: auto;

        font-size: 1rem;
    }

    .shortcut {
        display: flex;
        justify-content: space-between;

        &:not(:last-child) {
            border-bottom: 1px solid var(--bg-muted);
            padding-bottom: 0.5em;
        }
    }

    .keys {
        font-family: monospace;
    }

    kbd {
        background-color: var(--bg-soft);
        border: 1px solid var(--bg-muted);
        border-radius: 3px;
        box-shadow: 0 1px 0 rgba(0, 0, 0, 0.2);
        color: var(--fg-text);
        display: inline-block;
        font-size: 0.85em;
        font-weight: 700;
        line-height: 1;
        padding: 0.2em 0.6em;
        white-space: nowrap;
    }

    p, span {
        color: var(--fg-text);
    }
</style>
