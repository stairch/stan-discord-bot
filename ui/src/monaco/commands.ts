import * as monaco from "monaco-editor";

type Editor = monaco.editor.IStandaloneCodeEditor;

export const initialiseMonacoCommands = (editor: Editor) => {
    addBoldCommand(editor);
    addItalicCommand(editor);
    addLinkCommand(editor);
    redUnderlineForTranslatedWithDeepl(editor);
};

const redUnderlineForTranslatedWithDeepl = (editor: Editor) => {
    // on paste, check if the text contains "Translated with DeepL.com (free version)"
    // if so, mark it as a warning

    const checkText = () => {
        const matches =
            editor
                .getModel()
                ?.findMatches(
                    "\n\nTranslated with DeepL.com (free version)",
                    true,
                    false,
                    false,
                    null,
                    true
                )
                .map((match) => ({
                    startLineNumber: match.range.startLineNumber,
                    startColumn: match.range.startColumn,
                    endLineNumber: match.range.endLineNumber,
                    endColumn: match.range.endColumn,
                    message:
                        "Are you sure you want to include this in your post?",
                    severity: monaco.MarkerSeverity.Warning,
                })) ?? [];

        monaco.editor.setModelMarkers(editor.getModel()!, "deepl", matches);
    };

    editor.onDidPaste(checkText);
    editor.onDidChangeModelContent(checkText);

    monaco.languages.registerCodeActionProvider("markdown", {
        provideCodeActions: (model, range, context, token) => {
            const actions = context.markers.map((marker) => {
                return {
                    title: `Remove this watermark`,
                    diagnostics: [marker],
                    kind: "quickfix",
                    edit: {
                        edits: [
                            {
                                resource: model.uri,
                                textEdit: {
                                    range: {
                                        startLineNumber: marker.startLineNumber,
                                        startColumn: marker.startColumn,
                                        endLineNumber: marker.endLineNumber,
                                        endColumn: marker.endColumn,
                                    },
                                    text: "",
                                },
                            },
                        ],
                    },
                    isPreferred: true,
                };
            });
            return {
                actions: actions,
                dispose: checkText,
            } as monaco.languages.CodeActionList;
        },
    });
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
