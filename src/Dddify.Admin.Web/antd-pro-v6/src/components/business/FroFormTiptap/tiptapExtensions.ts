import Link from '@tiptap/extension-link';
import Placeholder from '@tiptap/extension-placeholder';
import TextAlign from '@tiptap/extension-text-align';
import StarterKit from '@tiptap/starter-kit';

export const createFroFormTiptapExtensions = (placeholder?: string) => [
  StarterKit.configure({
    code: false,
    codeBlock: false,
    heading: {
      levels: [1, 2, 3],
    },
  }),
  Link.configure({
    autolink: true,
    openOnClick: false,
  }),
  TextAlign.configure({
    types: ['heading', 'paragraph'],
  }),
  Placeholder.configure({
    placeholder,
  }),
];
