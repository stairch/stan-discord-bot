export const actualMarkdownToMarkdownV2 = (text: string) => {
    // Convert actual markdown to Telegram/WhatsApp-required MarkdownV2
    text = text.replace(/(?<!\*)\*(?!\*)/g, "_");
    text = text.replace(/\*\*/g, "*");
    text = text.replace(/([!\-.()])/g, "$1");
    return text;
};
