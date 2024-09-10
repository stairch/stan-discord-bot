import * as monaco from "monaco-editor";

type Editor = monaco.editor.IStandaloneCodeEditor;

export const initialiseMonacoCommands = (editor: Editor) => {
    addBoldCommand(editor);
    addItalicCommand(editor);
    addLinkCommand(editor);
};

const addBoldCommand = (editor: Editor) => {
    editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyB, () => {
        const selection = editor.getSelection();
        if (!selection) return;
        const text = editor.getModel()?.getValueInRange(selection);
        if (!text) return;
        if (text.startsWith("**") && text.endsWith("**")) {
            editor.executeEdits("", [
                {
                    range: selection,
                    text: text.slice(2, -2),
                },
            ]);
        } else {
            editor.executeEdits("", [
                {
                    range: selection,
                    text: `**${text}**`,
                },
            ]);
        }
    });
};

const addItalicCommand = (editor: Editor) => {
    editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyI, () => {
        const selection = editor.getSelection();
        if (!selection) return;
        const text = editor.getModel()?.getValueInRange(selection);
        if (!text) return;
        if (text.startsWith("_") && text.endsWith("_")) {
            editor.executeEdits("", [
                {
                    range: selection,
                    text: text.slice(1, -1),
                },
            ]);
        } else {
            editor.executeEdits("", [
                {
                    range: selection,
                    text: `_${text}_`,
                },
            ]);
        }
    });
};

const addLinkCommand = (editor: Editor) => {
    editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyK, async () => {
        const selection = editor.getSelection();
        if (!selection) return;
        const text = editor.getModel()?.getValueInRange(selection);
        if (!text) return;

        const clipboardText = await navigator.clipboard.readText();
        let link = clipboardText.startsWith("http") ? clipboardText : "";

        if (text.startsWith("[") && text.endsWith("]")) {
            editor.executeEdits("", [
                {
                    range: selection,
                    text: text.slice(1, -1),
                },
            ]);
        } else {
            editor.executeEdits("", [
                {
                    range: selection,
                    text: `[${text}](${link})`,
                },
            ]);
        }
    });
};
