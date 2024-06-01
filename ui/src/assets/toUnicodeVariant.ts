/**
 * (c) David Konrad 2018-
 * MIT License
 *
 * Javascript function to convert plain text to unicode variants
 *
 * For more inspiration see http://unicode.org/charts/
 *
 * Contributors
 *
 * @anhdung98 https://github.com/anhdung98
 * @degola https://github.com/degola
 *
 */

const VARIANTS = {
    monospace: "m",
    bold: "b",
    italic: "i",
    "bold italic": "bi",
    script: "c",
    "bold script": "bc",
    gothic: "g",
    "gothic bold": "bg",
    doublestruck: "d",
    sans: "s",
    "bold sans": "bs",
    "italic sans": "is",
    "bold italic sans": "bis",
    parenthesis: "p",
    circled: "o",
    "circled negative": "on",
    squared: "q",
    "squared negative": "qn",
    fullwidth: "w",
    //
    flags: "f",
    "numbers dot": "nd",
    "numbers comma": "nc",
    "numbers double circled": "ndc",
    roman: "r",
};

const COMBININGS = {
    strike: { short: "s", code: 0x0336 },
    "strike-curly": { short: "sc", code: 0x0334 },
    underline: { short: "u", code: 0x0332 },
    "underline-curly": { short: "uc", code: 0x0330 },
    "underline-sm": { short: "u-sm", code: 0x0320 },
    "underline-double": { short: "ud", code: 0x0333 },
    "underline-double-sm": { short: "ud-sm", code: 0x0347 },
    overline: { short: "o", code: 0x0305 },
    "overline-curly": { short: "oc", code: 0x0303 },
    "overline-sm": { short: "o-sm", code: 0x0304 },
    "overline-double": { short: "od", code: 0x033f },
    slash: { short: "sl", code: 0x0338 },
    "cross-above": { short: "ca", code: 0x033d },
    "plus-below": { short: "pb", code: 0x031f },
    "a-above": { short: "a-a", code: 0x0363 },
    "c-above": { short: "c-a", code: 0x0368 },
    "d-above": { short: "d-a", code: 0x0369 },
    "e-above": { short: "e-a", code: 0x0364 },
    "h-above": { short: "h-a", code: 0x036a },
    "i-above": { short: "i-a", code: 0x0365 },
    "m-above": { short: "m-a", code: 0x036b },
    "o-above": { short: "o-a", code: 0x0366 },
    "r-above": { short: "r-a", code: 0x036c },
    "u-above": { short: "u-a", code: 0x0367 },
    "v-above": { short: "v-a", code: 0x036e },
    "x-above": { short: "x-a", code: 0x036f },
    "halo-breve": { short: "hb", code: 0x0488 },
    "halo-grave": { short: "hg", code: 0x0489 },
    "enclose-circle": { short: "en-c", code: 0x20dd },
    "enclose-backslash": { short: "en-cb", code: 0x20e0 },
    "enclose-circle-backslash": { short: "en-cb", code: 0x20e0 },
    "enclose-square": { short: "en-s", code: 0x20de },
    "enclose-diamond": { short: "en-d", code: 0x20df },
    "enclose-screen": { short: "en-scr", code: 0x20e2 },
    "enclose-keycap": { short: "en-key", code: 0x20e3 },
    //COMBININGS supporting special chars
    diaeresis: { code: 0x0308 },
    caron: { code: 0x030c },
    perispomeni: { code: 0x0342 },
    tilde: { code: 0x0303 },
    tildesm: { code: 0x02dc },
    circumflex: { code: 0x0302 },
    ringabove: { code: 0x030a },
    dotabove: { code: 0x0307 },
    dotbelow: { code: 0x0323 },
    grave: { code: 0x0340 },
    gravedouble: { code: 0x030f },
    acute: { code: 0x0341 },
    breve: { code: 0x0306 },
    breveinverted: { code: 0x0311 },
    commabelow: { code: 0x0326 },
    macron: { code: 0x0304 },
    cedilla: { code: 0x0327 },
    ogonek: { code: 0x0328 },
    solidus: { code: 0x0338 },
    solidussm: { code: 0x0337 },
    hookabove: { code: 0x0309 },
    horn: { code: 0x031b },
    //spacing combinings
    "space-zero": { code: 0xfeff },
    "space-hair": { code: 0x200a },
    "space-thin": { code: 0x2009 },
    space: { code: 0x0020 },
    "space-en": { code: 0x2000 },
    "space-figure": { code: 0x2007 },
    "space-cjk": { code: 0x3000 },
    "space-em": { code: 0x2001 },
    "space-ogham": { code: 0x1680 },
    //combining grapheme joiner
    CGJ: { code: 0x034f },
};

export type UnicodeVariant = keyof typeof VARIANTS;
export type Combining = keyof typeof COMBININGS;

export function toUnicodeVariant(
    str: String | Number,
    variant: UnicodeVariant,
    combinings: Combining[] = []
): string {
    const string = String.fromCodePoint;

    const offsets = {
        m: [0x1d670, 0x1d7f6],
        b: [0x1d400, 0x1d7ce],
        i: [0x1d434, 0x00030],
        bi: [0x1d468, 0x00030],
        c: [0x0001d49c, 0x00030],
        bc: [0x1d4d0, 0x00030],
        g: [0x1d504, 0x00030],
        d: [0x1d538, 0x1d7d8],
        bg: [0x1d56c, 0x00030],
        s: [0x1d5a0, 0x1d7e2],
        bs: [0x1d5d4, 0x1d7ec],
        is: [0x1d608, 0x00030],
        bis: [0x1d63c, 0x00030],
        o: [0x24b6, 0x245f],
        on: [0x0001f150, 0x245f],
        p: [0x1f110, 0x1d7f6],
        q: [0x1f130, 0x00030],
        qn: [0x0001f170, 0x00030],
        w: [0xff21, 0xff10],
        //
        f: [0x1f1e6, 0x1d7f6],
        nd: [0x1d670, 0x2487],
        nc: [0x1d670, 0x1f101],
        ndc: [0x1d670, 0x24f4],
        r: [0x1d670, 0x24f4],
    };

    const special: any = {
        m: {
            " ": 0x2000,
            "-": 0x2013,
        },
        i: {
            h: 0x210e,
        },
        c: {
            B: 0x212c,
            E: 0x2130,
            F: 0x2131,
            H: 0x210b,
            I: 0x2110,
            L: 0x2112,
            M: 0x2133,
            R: 0x211b,
            e: 0x1d4ee,
            g: 0x1d4f0,
            o: 0x1d4f8,
        },
        g: {
            C: 0x212d,
            H: 0x210c,
            I: 0x2111,
            R: 0x211c,
            Z: 0x2128,
        },
        d: {
            C: 0x2102,
            H: 0x210d,
            N: 0x2115,
            P: 0x2119,
            Q: 0x211a,
            R: 0x211d,
            Z: 0x2124,
        },
        o: {
            "0": 0x24ea,
            "10": 0x2469,
            "11": 0x246a,
            "12": 0x246b,
            "13": 0x246c,
            "14": 0x246d,
            "15": 0x246e,
            "16": 0x246f,
            "17": 0x2470,
            "18": 0x2471,
            "19": 0x2472,
            "20": 0x2473,
        },
        on: {
            "0": 0x24ff,
            "11": 0x24eb,
            "12": 0x24ec,
            "13": 0x24ed,
            "14": 0x24ee,
            "15": 0x24ef,
            "16": 0x24f0,
            "17": 0x24f1,
            "18": 0x24f2,
            "19": 0x24f3,
            "20": 0x24f4,
        },
        p: {
            "1": 0x2474,
            "2": 0x2475,
            "3": 0x2476,
            "4": 0x2477,
            "5": 0x2478,
            "6": 0x2479,
            "7": 0x247a,
            "8": 0x247b,
            "9": 0x247c,
            "10": 0x247d,
            "11": 0x247e,
            "12": 0x247f,
            "13": 0x2480,
            "14": 0x2481,
            "15": 0x2482,
            "16": 0x2483,
            "17": 0x2484,
            "18": 0x2485,
            "19": 0x2486,
            "20": 0x2487,
        },
        q: {
            hv: 0x1f14a,
            mv: 0x1f14b,
            sd: 0x1f14c,
            ss: 0x1f14d,
            ppv: 0x1f14e,
            wc: 0x1f14f,
            cl: 0x1f191,
            cool: 0x1f192,
            free: 0x1f193,
            id: 0x1f194,
            new: 0x1f195,
            ng: 0x1f196,
            ok: 0x1f197,
            sos: 0x1f198,
            "up!": 0x1f199,
            vs: 0x1f19a,
            "3d": 0x1f19b,
            "2ndscr": 0x1f19c,
            "2k": 0x1f19d,
            "4k": 0x1f19e,
            "8k": 0x1f19f,
            "5.1": 0x1f1a0,
            "7.1": 0x1f1a1,
            "22.2": 0x1f1a2,
            "60p": 0x1f1a3,
            "120p": 0x1f1a4,
            d: 0x1f1a5,
            hc: 0x1f1a6,
            hdr: 0x1f1a7,
            "hi-res": 0x1f1a8,
            "loss-less": 0x1f1a9,
            shv: 0x1f1aa,
            uhd: 0x1f1ab,
            vod: 0x1f1ac,
        },
        qn: {
            ic: 0x1f18b,
            pa: 0x1f18c,
            sa: 0x1f18d,
            ab: 0x1f18e,
            wc: 0x1f18f,
        },
        w: {
            "!": 0xff01,
            '"': 0xff02,
            "#": 0xff03,
            $: 0xff04,
            "%": 0xff05,
            "&": 0xff06,
            "'": 0xff07,
            "(": 0xff08,
            ")": 0xff09,
            "*": 0xff0a,
            "+": 0xff0b,
            ",": 0xff0c,
            "-": 0xff0d,
            ".": 0xff0e,
            "/": 0xff0f,
            ":": 0xff1a,
            ";": 0xff1b,
            "<": 0xff1c,
            "=": 0xff1d,
            ">": 0xff1e,
            "?": 0xff1f,
            "@": 0xff20,
            "\\": 0xff3c,
            "[": 0xff3b,
            "]": 0xff3d,
            "^": 0xff3e,
            _: 0xff3f,
            "`": 0xff40,
            "{": 0xff5b,
            "|": 0xff5c,
            "}": 0xff5d,
            "~": 0xff5e,
            "⦅": 0xff5f,
            "⦆": 0xff60,
            "￠": 0xffe0,
            "￡": 0xffe1,
            "¦": 0xffe4,
            "￥": 0xffe5,
            "￦": 0xffe6,
            ｰ: 0xff70,
            "｡": 0xff70,
            "､": 0xff64,
            "･": 0xff65,
            "￣": 0xffe3,
            "¬": 0xffe2,
        },
        f: {},
        nd: {
            "0": 0x1f100,
            "10": 0x2491,
            "11": 0x2492,
            "12": 0x2493,
            "13": 0x2494,
            "14": 0x2495,
            "15": 0x2496,
            "16": 0x2497,
            "17": 0x2498,
            "18": 0x2499,
            "19": 0x249a,
            "20": 0x249b,
        },
        ndc: {
            "0": 0x1d7f6,
            "10": 0x24fe,
        },
        r: {
            I: 0x2160,
            II: 0x2161,
            III: 0x2162,
            IV: 0x2163,
            V: 0x2164,
            VI: 0x2165,
            VII: 0x2166,
            VIII: 0x2167,
            IX: 0x2168,
            X: 0x2169,
            XI: 0x216a,
            XII: 0x216b,
            L: 0x216c,
            C: 0x216d,
            D: 0x216e,
            M: 0x216f,
            i: 0x2170,
            ii: 0x2171,
            iii: 0x2172,
            iv: 0x2173,
            v: 0x2174,
            vi: 0x2175,
            vii: 0x2176,
            viii: 0x2177,
            ix: 0x2178,
            x: 0x2179,
            xi: 0x217a,
            xii: 0x217b,
            l: 0x217c,
            c: 0x217d,
            d: 0x217e,
            m: 0x217f,
        },
    };

    //paranthesis, support small letters
    //fullwidth, support small letters
    for (var i = 97; i <= 122; i++) {
        special["p"][String.fromCharCode(i)] = 0x249c + (i - 97);
        special["w"][String.fromCharCode(i)] = 0xff41 + (i - 97);
    }

    //circled negative, support small letters
    //squared, support small letters
    //squared negative, support small letters
    (["on", "q", "qn", "f"] as any as UnicodeVariant[]).forEach((t) => {
        for (var i = 97; i <= 122; i++) {
            special[t][String.fromCharCode(i)] =
                (offsets as any)[t][0] + (i - 97);
        }
    });

    const special_chars = {
        ä: { char: "a", combine: string(COMBININGS.diaeresis.code) },
        â: { char: "a", combine: string(COMBININGS.circumflex.code) },
        á: { char: "a", combine: string(COMBININGS.acute.code) },
        å: { char: "a", combine: string(COMBININGS.ringabove.code) },
        ă: { char: "a", combine: string(COMBININGS.breve.code) },
        ǟ: {
            char: "a",
            combine:
                string(COMBININGS.diaeresis.code) +
                string(COMBININGS.macron.code),
        },
        ã: { char: "a", combine: string(COMBININGS.tilde.code) },
        ā: { char: "a", combine: string(COMBININGS.macron.code) },
        ȧ: { char: "a", combine: string(COMBININGS.dotabove.code) },
        ȃ: { char: "a", combine: string(COMBININGS.breveinverted.code) },
        ḅ: { char: "b", combine: string(COMBININGS.dotbelow.code) },
        č: { char: "c", combine: string(COMBININGS.caron.code) },
        ć: { char: "c", combine: string(COMBININGS.acute.code) },
        ç: { char: "c", combine: string(COMBININGS.cedilla.code) },
        ḉ: {
            char: "c",
            combine:
                string(COMBININGS.cedilla.code) + string(COMBININGS.acute.code),
        },
        ċ: { char: "c", combine: string(COMBININGS.dotabove.code) },
        ĉ: { char: "c", combine: string(COMBININGS.circumflex.code) },
        è: { char: "e", combine: string(COMBININGS.grave.code) },
        é: { char: "e", combine: string(COMBININGS.acute.code) },
        ē: { char: "e", combine: string(COMBININGS.macron.code) },
        ĕ: { char: "e", combine: string(COMBININGS.breve.code) },
        ë: { char: "e", combine: string(COMBININGS.diaeresis.code) },
        ě: { char: "e", combine: string(COMBININGS.caron.code) },
        ę: { char: "e", combine: string(COMBININGS.ogonek.code) },
        ȇ: { char: "e", combine: string(COMBININGS.breveinverted.code) },
        ȅ: { char: "e", combine: string(COMBININGS.gravedouble.code) },
        ê: { char: "e", combine: string(COMBININGS.circumflex.code) },
        ğ: { char: "g", combine: string(COMBININGS.breve.code) },
        ǧ: { char: "g", combine: string(COMBININGS.caron.code) },
        ģ: { char: "g", combine: string(COMBININGS.cedilla.code) },
        ġ: { char: "g", combine: string(COMBININGS.dotabove.code) },
        ḥ: { char: "h", combine: string(COMBININGS.dotbelow.code) },
        î: { char: "i", combine: string(COMBININGS.circumflex.code) },
        í: { char: "i", combine: string(COMBININGS.acute.code) },
        ì: { char: "i", combine: string(COMBININGS.grave.code) },
        ĩ: { char: "i", combine: string(COMBININGS.tilde.code) },
        ḱ: { char: "k", combine: string(COMBININGS.acute.code) },
        ḳ: { char: "k", combine: string(COMBININGS.dotbelow.code) },
        ņ: { char: "n", combine: string(COMBININGS.ogonek.code) },
        ń: { char: "n", combine: string(COMBININGS.acute.code) },
        õ: { char: "o", combine: string(COMBININGS.tilde.code) },
        ö: { char: "o", combine: string(COMBININGS.diaeresis.code) },
        ō: { char: "o", combine: string(COMBININGS.macron.code) },
        ô: { char: "o", combine: string(COMBININGS.circumflex.code) },
        ó: { char: "o", combine: string(COMBININGS.acute.code) },
        ò: { char: "o", combine: string(COMBININGS.grave.code) },
        ŕ: { char: "r", combine: string(COMBININGS.acute.code) },
        ş: { char: "s", combine: string(COMBININGS.cedilla.code) },
        ș: { char: "s", combine: string(COMBININGS.commabelow.code) },
        ṩ: {
            char: "s",
            combine:
                string(COMBININGS.dotbelow.code) +
                string(COMBININGS.dotabove.code),
        },
        š: { char: "s", combine: string(COMBININGS.caron.code) },
        ś: { char: "s", combine: string(COMBININGS.acute.code) },
        ü: { char: "u", combine: string(COMBININGS.diaeresis.code) },
        ù: { char: "u", combine: string(COMBININGS.grave.code) },
        ú: { char: "u", combine: string(COMBININGS.acute.code) },
        û: { char: "u", combine: string(COMBININGS.circumflex.code) },
        ŭ: { char: "u", combine: string(COMBININGS.breve.code) },
        ȕ: { char: "u", combine: string(COMBININGS.gravedouble.code) },
        ȗ: { char: "u", combine: string(COMBININGS.breveinverted.code) },
        ů: { char: "u", combine: string(COMBININGS.ringabove.code) },
        ū: { char: "u", combine: string(COMBININGS.macron.code) },
        ẁ: { char: "w", combine: string(COMBININGS.grave.code) },
        ẃ: { char: "w", combine: string(COMBININGS.acute.code) },
        ø: { char: "o", combine: string(COMBININGS.solidussm.code) },
        //mimicks that somehow fails
        //'c̆': { 'char': 'c', 'combine': string(COMBININGS.breve.code) },
        //'c̈': { 'char': 'c', 'combine': string(COMBININGS.diaeresis.code) },
        //'ę́': { 'char': 'e', 'combine': string(COMBININGS.ogonek.code) + string(COMBININGS.acute.code) },
        //'m̂': { 'char': 'm', 'combine': string(COMBININGS.circumflex.code) },
        //'n̂': { 'char': 'n', 'combine': string(COMBININGS.circumflex.code) },
        //'ñ': { 'char': 'n', 'combine': string(COMBININGS.tilde.code) },
        //'q̄': { 'char': 'q', 'combine': string(COMBININGS.macron.code) },
        //'r̃': { 'char': 'r', 'combine': string(COMBININGS.tilde.code) },
        //'s̈': { 'char': 's', 'combine': string(COMBININGS.diaeresis.code) },
        đ: { char: "d", combine: string(COMBININGS.macron.code) },
        à: { char: "a", combine: string(COMBININGS.grave.code) },
        ả: { char: "a", combine: string(COMBININGS.hookabove.code) },
        ạ: { char: "a", combine: string(COMBININGS.dotbelow.code) },
        ẻ: { char: "e", combine: string(COMBININGS.hookabove.code) },
        ẽ: { char: "e", combine: string(COMBININGS.tilde.code) },
        ẹ: { char: "e", combine: string(COMBININGS.dotbelow.code) },
        ỉ: { char: "i", combine: string(COMBININGS.hookabove.code) },
        ị: { char: "i", combine: string(COMBININGS.dotbelow.code) },
        ỏ: { char: "o", combine: string(COMBININGS.hookabove.code) },
        ọ: { char: "o", combine: string(COMBININGS.dotbelow.code) },
        ủ: { char: "u", combine: string(COMBININGS.hookabove.code) },
        ũ: { char: "u", combine: string(COMBININGS.tilde.code) },
        ụ: { char: "u", combine: string(COMBININGS.dotbelow.code) },
        ý: { char: "y", combine: string(COMBININGS.acute.code) },
        ỳ: { char: "y", combine: string(COMBININGS.grave.code) },
        ỷ: { char: "y", combine: string(COMBININGS.hookabove.code) },
        ỹ: { char: "y", combine: string(COMBININGS.tilde.code) },
        ỵ: { char: "y", combine: string(COMBININGS.dotbelow.code) },
        ắ: {
            char: "a",
            combine:
                string(COMBININGS.breve.code) + string(COMBININGS.acute.code),
        },
        ằ: {
            char: "a",
            combine:
                string(COMBININGS.breve.code) + string(COMBININGS.grave.code),
        },
        ẳ: {
            char: "a",
            combine:
                string(COMBININGS.breve.code) +
                string(COMBININGS.hookabove.code),
        },
        ẵ: {
            char: "a",
            combine:
                string(COMBININGS.breve.code) + string(COMBININGS.tilde.code),
        },
        ặ: {
            char: "a",
            combine:
                string(COMBININGS.breve.code) +
                string(COMBININGS.dotbelow.code),
        },
        ấ: {
            char: "a",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.acute.code),
        },
        ầ: {
            char: "a",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.grave.code),
        },
        ẩ: {
            char: "a",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.hookabove.code),
        },
        ẫ: {
            char: "a",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.tilde.code),
        },
        ậ: {
            char: "a",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.dotbelow.code),
        },
        ế: {
            char: "e",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.acute.code),
        },
        ề: {
            char: "e",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.grave.code),
        },
        ể: {
            char: "e",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.hookabove.code),
        },
        ễ: {
            char: "e",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.tilde.code),
        },
        ệ: {
            char: "e",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.dotbelow.code),
        },
        ố: {
            char: "o",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.acute.code),
        },
        ồ: {
            char: "o",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.grave.code),
        },
        ổ: {
            char: "o",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.hookabove.code),
        },
        ỗ: {
            char: "o",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.tilde.code),
        },
        ộ: {
            char: "o",
            combine:
                string(COMBININGS.circumflex.code) +
                string(COMBININGS.dotbelow.code),
        },
        ơ: { char: "o", combine: string(COMBININGS.horn.code) },
        ớ: {
            char: "o",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.acute.code),
        },
        ờ: {
            char: "o",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.grave.code),
        },
        ở: {
            char: "o",
            combine:
                string(COMBININGS.horn.code) +
                string(COMBININGS.hookabove.code),
        },
        ỡ: {
            char: "o",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.tilde.code),
        },
        ợ: {
            char: "o",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.dotbelow.code),
        },
        ư: { char: "u", combine: string(COMBININGS.horn.code) },
        ứ: {
            char: "u",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.acute.code),
        },
        ừ: {
            char: "u",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.grave.code),
        },
        ử: {
            char: "u",
            combine:
                string(COMBININGS.horn.code) +
                string(COMBININGS.hookabove.code),
        },
        ữ: {
            char: "u",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.tilde.code),
        },
        ự: {
            char: "u",
            combine:
                string(COMBININGS.horn.code) + string(COMBININGS.dotbelow.code),
        },
    };

    //reset special chars, capital letters
    //in the future, some capital speciel chars can be mimicked as well
    for (const char of Object.keys(special_chars)) {
        (special_chars as any)[char.toUpperCase()] = {
            char: char,
            combine: false,
        };
    }

    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    const numbers = "0123456789";

    const type = (function () {
        if (VARIANTS[variant]) return VARIANTS[variant];
        if ((offsets as any)[variant]) return variant;
        return "m"; //monospace as default
    })();

    const combine_with = (function () {
        let result = "";
        combinings.forEach((diacritic) => {
            diacritic = diacritic.trim().toLowerCase() as Combining;
            for (const d in COMBININGS) {
                if (
                    diacritic === d ||
                    diacritic === (COMBININGS as any)[d].short
                ) {
                    result += string((COMBININGS as any)[d].code); //+ string(COMBININGS.CGJ.code) seem not to have any effect
                }
            }
        });
        return result;
    })();

    //if entire sequence is supported
    if (
        typeof str === "string" &&
        special[type] &&
        (special[type][str] || special[type][str.toLowerCase()])
    ) {
        return special[type][str]
            ? string(special[type][str])
            : string(special[type][str.toLowerCase()]);
    }

    //support for romanization
    if (["roman", "r"].includes(type)) {
        if (typeof str === "number") {
            //-- based on https://blog.stevenlevithan.com/archives/javascript-roman-numeral-converter
            const parts = {
                M: 1000,
                CM: 900,
                D: 500,
                CD: 400,
                C: 100,
                XC: 90,
                L: 50,
                XL: 40,
                X: 10,
                IX: 9,
                V: 5,
                IV: 4,
                I: 1,
            };
            let roman = "";
            let num = str;
            for (let i in parts) {
                while (num >= (parts as any)[i]) {
                    if (special[type][i]) {
                        roman += i;
                    } else {
                        for (let d of i) roman += d;
                    }
                    num -= (parts as any)[i];
                }
            }
            str = roman;
        }
        let result = str;
        const romans = [
            "VIII",
            "viii",
            "III",
            "iii",
            "XII",
            "xii",
            "VII",
            "vii",
            "IX",
            "ix",
            "XI",
            "xi",
            "IV",
            "iv",
            "VI",
            "vi",
            "II",
            "ii",
            "I",
            "i",
            "D",
            "d",
            "M",
            "m",
            "L",
            "l",
            "V",
            "v",
            "C",
            "c",
            "X",
            "x",
        ];
        for (let number of romans) {
            if (typeof result !== "string") {
                result = String(result);
            }

            if (result.indexOf(number.toString()) > -1) {
                result = result.replaceAll(
                    number,
                    string(special[type][number])
                );
            }
        }
        return String(result);
    }

    let result = "";

    for (let c of String(str)) {
        let index;
        const combine_special =
            c in special_chars ? (special_chars as any)[c].combine : false;
        c = combine_special
            ? (special_chars as any)[c].char
            : c.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
        if (special[type] && special[type][c]) c = string(special[type][c]);
        if (type && (index = chars.indexOf(c)) > -1) {
            result += string(index + (offsets as any)[type][0]);
        } else if (type && (index = numbers.indexOf(c)) > -1) {
            result += string(index + (offsets as any)[type][1]);
        } else {
            result += c;
        }
        if (combine_special) result += combine_special;
        if (combine_with) result += combine_with;
    }

    return result;
}
