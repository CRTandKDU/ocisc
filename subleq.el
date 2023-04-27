;; This buffer is for text that is not saved, and for Lisp evaluation.
;; To create a file, visit it with C-x C-f and enter text in its buffer.

(defun subleq-tricolumns ()
  "Display list of CR-separated numbers in region in a 3-column
SUBLEQ format."
    (interactive)
  (goto-char (region-beginning))
  (while (not (eobp))
    (end-of-line)
    (insert " ")
    (delete-char 1)
    (end-of-line)
    (insert " ")
    (delete-char 1)
    (beginning-of-line 2))
  )

    
   
