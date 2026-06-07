import type { Extensions } from '@tiptap/core';
import { EditorContent, useEditor } from '@tiptap/react';
import type { CSSProperties, ReactNode } from 'react';
import { useEffect, useMemo } from 'react';
import { createFroFormTiptapExtensions } from '../FroFormTiptap/tiptapExtensions';
import { useFroFormTiptapStyles } from '../FroFormTiptap/tiptapStyles';

export interface FroFormTiptapPreviewProps {
  /** 待预览的 HTML 内容，保留 value 命名以便与表单字段保持一致。 */
  value?: string;
  /** 待预览的 HTML 内容；优先级高于 value，适合非表单场景。 */
  content?: string;
  /** 是否显示边框容器。 */
  bordered?: boolean;
  /** 内容为空时显示的占位内容。 */
  emptyText?: ReactNode;
  /** 预览区域最小高度，数字会按 px 处理。 */
  minHeight?: number | string;
  /** 预览区域最大高度，超出后内部滚动，数字会按 px 处理。 */
  maxHeight?: number | string;
  /** 自定义 Tiptap 扩展；传入后会替代默认扩展集合。 */
  extensions?: Extensions;
  /** 外层容器 className。 */
  className?: string;
  /** 外层容器 style。 */
  style?: CSSProperties;
}

const toCssSize = (value?: number | string) =>
  typeof value === 'number' ? `${value}px` : value;

const FroFormTiptapPreview = ({
  value,
  content,
  bordered = true,
  emptyText = '-',
  minHeight = 160,
  maxHeight,
  extensions,
  className,
  style,
}: FroFormTiptapPreviewProps) => {
  const { styles, cx } = useFroFormTiptapStyles();
  const html = content ?? value ?? '';
  const mergedExtensions = useMemo(
    () => extensions ?? createFroFormTiptapExtensions(),
    [extensions],
  );
  const editor = useEditor({
    extensions: mergedExtensions,
    content: html,
    editable: false,
    immediatelyRender: false,
  });
  const contentStyle = {
    '--fro-form-tiptap-min-height': toCssSize(minHeight),
    maxHeight: toCssSize(maxHeight),
    overflowY: maxHeight ? 'auto' : undefined,
  } as CSSProperties;

  useEffect(() => {
    editor?.commands.setContent(html, { emitUpdate: false });
  }, [editor, html]);

  if (!html) {
    return (
      <div
        className={cx(
          styles.preview,
          !bordered && styles.previewPlain,
          className,
        )}
        style={style}
      >
        <div className={styles.empty}>{emptyText}</div>
      </div>
    );
  }

  return (
    <div
      className={cx(styles.preview, !bordered && styles.previewPlain, className)}
      style={style}
    >
      <EditorContent
        editor={editor}
        className={styles.content}
        style={contentStyle}
      />
    </div>
  );
};

export default FroFormTiptapPreview;
