(require 'gforth)
;; Highlights :t ;t target word definitions and :a ;a SUBLEQ assembly
(setq forth-custom-words
      '(((":a") definition-starter (font-lock-keyword-face . 1)
         "[ \t\n]" t name (font-lock-function-name-face . 3))
	((";a") definition-ender (font-lock-keyword-face . 1))
	((":t") definition-starter (font-lock-keyword-face . 1)
         "[ \t\n]" t name (font-lock-function-name-face . 3))
	((";t") definition-ender (font-lock-keyword-face . 1))))
