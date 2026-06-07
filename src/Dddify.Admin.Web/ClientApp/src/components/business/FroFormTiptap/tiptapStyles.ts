import { createStyles } from 'antd-style';

export const useFroFormTiptapStyles = createStyles(({ token }) => ({
  shell: {
    border: `1px solid ${token.colorBorderSecondary}`,
    borderRadius: token.borderRadius,
    background: token.colorBgContainer,
    transition: `border-color ${token.motionDurationMid}, box-shadow ${token.motionDurationMid}`,
    '&:focus-within': {
      borderColor: token.colorPrimary,
      boxShadow: `0 0 0 1px ${token.colorBorderSecondary}`,
    },
  },
  shellDisabled: {
    background: token.colorBgContainerDisabled,
    color: token.colorTextDisabled,
    '&:focus-within': {
      borderColor: token.colorBorder,
      boxShadow: 'none',
    },
  },
  toolbar: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: 4,
    alignItems: 'center',
    minHeight: 40,
    padding: '4px 7px',
    borderBottom: `1px solid ${token.colorBorderSecondary}`,
    background: token.colorFillAlter,
    borderStartStartRadius: token.borderRadius,
    borderStartEndRadius: token.borderRadius,
  },
  toolbarSpacer: {
    flex: 1,
    minWidth: 8,
  },
  toolbarDivider: {
    width: 1,
    height: 22,
    marginInline: 2,
    background: token.colorBorderSecondary,
  },
  blockButton: {
    minWidth: 82,
    justifyContent: 'space-between',
  },
  linkPopover: {
    width: 280,
  },
  content: {
    padding: '12px 14px',
    color: token.colorText,
    '& .ProseMirror': {
      minHeight: 'var(--fro-form-tiptap-min-height, 180px)',
      outline: 'none',
      wordBreak: 'break-word',
    },
    '& .ProseMirror p': {
      marginBlock: '0 0 0.75em',
    },
    '& .ProseMirror p:last-child': {
      marginBlockEnd: 0,
    },
    '& .ProseMirror h1': {
      marginBlock: '0 0 0.65em',
      fontSize: 28,
      fontWeight: 600,
      lineHeight: 1.25,
    },
    '& .ProseMirror h2': {
      marginBlock: '0 0 0.65em',
      fontSize: 22,
      fontWeight: 600,
      lineHeight: 1.3,
    },
    '& .ProseMirror h3': {
      marginBlock: '0 0 0.65em',
      fontSize: 18,
      fontWeight: 600,
      lineHeight: 1.35,
    },
    '& .ProseMirror ul, & .ProseMirror ol': {
      marginBlock: '0 0 0.75em',
      paddingInlineStart: 24,
      listStylePosition: 'outside',
    },
    '& .ProseMirror ul': {
      listStyleType: 'disc',
    },
    '& .ProseMirror ol': {
      listStyleType: 'decimal',
    },
    '& .ProseMirror li': {
      paddingInlineStart: 2,
      display: 'list-item',
    },
    '& .ProseMirror li + li': {
      marginBlockStart: 4,
    },
    '& .ProseMirror blockquote': {
      margin: '0 0 0.75em',
      padding: '2px 0 2px 12px',
      color: token.colorTextSecondary,
      borderInlineStart: `3px solid ${token.colorBorder}`,
    },
    '& .ProseMirror a': {
      color: token.colorLink,
      textDecoration: 'underline',
      textUnderlineOffset: 2,
    },
    '& .ProseMirror.ProseMirror-focused a': {
      cursor: 'text',
    },
    '& .ProseMirror p.is-editor-empty:first-child::before': {
      content: 'attr(data-placeholder)',
      float: 'left',
      height: 0,
      color: token.colorTextQuaternary,
      pointerEvents: 'none',
    },
  },
  preview: {
    border: `1px solid ${token.colorBorderSecondary}`,
    borderRadius: token.borderRadius,
    background: token.colorBgContainer,
  },
  previewPlain: {
    border: 'none',
    borderRadius: 0,
    background: 'transparent',
  },
  empty: {
    padding: '12px 14px',
    color: token.colorTextQuaternary,
  },
}));
