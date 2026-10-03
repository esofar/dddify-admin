import {
  AlignCenterOutlined,
  AlignLeftOutlined,
  AlignRightOutlined,
  BoldOutlined,
  ClearOutlined,
  DownOutlined,
  EyeInvisibleOutlined,
  EyeOutlined,
  ItalicOutlined,
  LinkOutlined,
  OrderedListOutlined,
  RedoOutlined,
  StrikethroughOutlined,
  UndoOutlined,
  UnorderedListOutlined,
} from '@ant-design/icons';
import type { Extensions, JSONContent } from '@tiptap/core';
import type { Editor } from '@tiptap/react';
import { EditorContent, useEditor } from '@tiptap/react';
import type { MenuProps } from 'antd';
import { Button, Dropdown, Input, Popover, Space, Tooltip } from 'antd';
import type { CSSProperties, ReactNode } from 'react';
import { useEffect, useMemo, useState } from 'react';
import FroFormTiptapPreview from '../FroFormTiptapPreview';
import { createFroFormTiptapExtensions } from './tiptapExtensions';
import { useFroFormTiptapStyles } from './tiptapStyles';

export type FroFormTiptapChangeEvent = {
  /** 当前编辑器内容的 HTML 字符串，用于提交到后端或表单字段。 */
  html: string;
  /** 当前编辑器内容的 Tiptap JSON 结构，适合需要结构化存储时使用。 */
  json: JSONContent;
  /** 当前编辑器内容的纯文本，适合摘要、搜索索引或字数统计。 */
  text: string;
};

export interface FroFormTiptapProps {
  /** 受控 HTML 内容，通常由 Ant Design Form 接管。 */
  value?: string;
  /** 非受控初始 HTML 内容，仅初始化时生效。 */
  defaultValue?: string;
  /** 是否允许编辑内容；设置为 false 时进入只读模式但仍可显示工具栏。 */
  editable?: boolean;
  /** 是否禁用组件；禁用后不可编辑且展示禁用态样式。 */
  disabled?: boolean;
  /** 是否显示工具栏。 */
  toolbar?: boolean;
  /** 是否显示编辑/预览切换控件。 */
  previewable?: boolean;
  /** 编辑器为空时的占位提示。 */
  placeholder?: string;
  /** 编辑区域最小高度，数字会按 px 处理。 */
  minHeight?: number | string;
  /** 编辑区域最大高度，超出后内部滚动，数字会按 px 处理。 */
  maxHeight?: number | string;
  /** 自定义 Tiptap 扩展；传入后会替代默认扩展集合。 */
  extensions?: Extensions;
  /** 外层容器 className。 */
  className?: string;
  /** 外层容器 style。 */
  style?: CSSProperties;
  /** 内容变更回调；第一个参数兼容表单 value，第二个参数提供 html/json/text。 */
  onChange?: (value: string, event: FroFormTiptapChangeEvent) => void;
  /** 编辑器失焦回调。 */
  onBlur?: () => void;
}

type ToolbarButtonProps = {
  title: string;
  icon: ReactNode;
  active?: boolean;
  disabled?: boolean;
  onClick: () => void;
};

const EMPTY_HTML = '<p></p>';

const normalizeHtml = (html?: string) => html || EMPTY_HTML;

const toCssSize = (value?: number | string) =>
  typeof value === 'number' ? `${value}px` : value;

const emitChange = (
  editor: Editor,
  onChange?: FroFormTiptapProps['onChange'],
) => {
  const html = editor.getHTML();

  onChange?.(html, {
    html,
    json: editor.getJSON(),
    text: editor.getText(),
  });
};

const ToolbarButton = ({
  title,
  icon,
  active,
  disabled,
  onClick,
}: ToolbarButtonProps) => (
  <Tooltip title={title}>
    <Button
      aria-label={title}
      size="small"
      type={active ? 'primary' : 'text'}
      icon={icon}
      disabled={disabled}
      onClick={onClick}
    />
  </Tooltip>
);

const ToolbarDivider = () => {
  const { styles } = useFroFormTiptapStyles();

  return <span className={styles.toolbarDivider} />;
};

const getBlockLabel = (editor: Editor | null) => {
  if (!editor) {
    return '正文';
  }

  if (editor.isActive('heading', { level: 1 })) {
    return '标题 1';
  }

  if (editor.isActive('heading', { level: 2 })) {
    return '标题 2';
  }

  if (editor.isActive('heading', { level: 3 })) {
    return '标题 3';
  }

  return '正文';
};

const FroFormTiptapToolbar = ({
  editor,
  disabled,
  previewable,
  mode,
  onModeChange,
}: {
  editor: Editor | null;
  disabled?: boolean;
  previewable?: boolean;
  mode: 'edit' | 'preview';
  onModeChange: (mode: 'edit' | 'preview') => void;
}) => {
  const { styles } = useFroFormTiptapStyles();
  const [linkOpen, setLinkOpen] = useState(false);
  const [linkValue, setLinkValue] = useState('');
  const commandDisabled = disabled || !editor || mode === 'preview';

  const blockItems = useMemo<MenuProps['items']>(
    () => [
      { key: 'paragraph', label: '正文' },
      { key: 'heading-1', label: '标题 1' },
      { key: 'heading-2', label: '标题 2' },
      { key: 'heading-3', label: '标题 3' },
    ],
    [],
  );

  const run = (command: (editor: Editor) => void) => {
    if (!editor || commandDisabled) {
      return;
    }

    command(editor);
  };

  const handleBlockClick: MenuProps['onClick'] = ({ key }) => {
    run((currentEditor) => {
      if (key === 'paragraph') {
        currentEditor.chain().focus().setParagraph().run();
        return;
      }

      currentEditor
        .chain()
        .focus()
        .toggleHeading({
          level: Number(String(key).replace('heading-', '')) as 1 | 2 | 3,
        })
        .run();
    });
  };

  const openLinkPopover = (nextOpen: boolean) => {
    setLinkOpen(nextOpen);

    if (nextOpen && editor) {
      setLinkValue(editor.getAttributes('link').href ?? '');
    }
  };

  const applyLink = () => {
    run((currentEditor) => {
      const chain = currentEditor.chain().focus().extendMarkRange('link');
      const href = linkValue.trim();

      if (!href) {
        chain.unsetLink().run();
        return;
      }

      chain.setLink({ href }).run();
    });
    setLinkOpen(false);
  };

  return (
    <div className={styles.toolbar}>
      <Dropdown
        disabled={commandDisabled}
        menu={{ items: blockItems, onClick: handleBlockClick }}
        trigger={['click']}
      >
        <Button
          size="small"
          className={styles.blockButton}
          disabled={commandDisabled}
        >
          {getBlockLabel(editor)}
          <DownOutlined />
        </Button>
      </Dropdown>

      <ToolbarDivider />

      <ToolbarButton
        title="加粗"
        icon={<BoldOutlined />}
        active={editor?.isActive('bold')}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().toggleBold().run(),
          )
        }
      />
      <ToolbarButton
        title="斜体"
        icon={<ItalicOutlined />}
        active={editor?.isActive('italic')}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().toggleItalic().run(),
          )
        }
      />
      <ToolbarButton
        title="删除线"
        icon={<StrikethroughOutlined />}
        active={editor?.isActive('strike')}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().toggleStrike().run(),
          )
        }
      />

      <ToolbarDivider />

      <ToolbarButton
        title="无序列表"
        icon={<UnorderedListOutlined />}
        active={editor?.isActive('bulletList')}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().toggleBulletList().run(),
          )
        }
      />
      <ToolbarButton
        title="有序列表"
        icon={<OrderedListOutlined />}
        active={editor?.isActive('orderedList')}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().toggleOrderedList().run(),
          )
        }
      />
      <ToolbarButton
        title="引用"
        icon={<span style={{ fontWeight: 600 }}>""</span>}
        active={editor?.isActive('blockquote')}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().toggleBlockquote().run(),
          )
        }
      />

      <ToolbarDivider />

      <Popover
        trigger="click"
        open={linkOpen}
        onOpenChange={openLinkPopover}
        content={
          <Space.Compact className={styles.linkPopover}>
            <Input
              allowClear
              size="small"
              placeholder="https://example.com"
              value={linkValue}
              onChange={(event) => setLinkValue(event.target.value)}
              onPressEnter={applyLink}
            />
            <Button size="small" type="primary" onClick={applyLink}>
              确定
            </Button>
          </Space.Compact>
        }
      >
        <Tooltip title="链接">
          <Button
            aria-label="链接"
            size="small"
            type={editor?.isActive('link') ? 'primary' : 'text'}
            icon={<LinkOutlined />}
            disabled={commandDisabled}
          />
        </Tooltip>
      </Popover>

      <ToolbarDivider />

      <ToolbarButton
        title="左对齐"
        icon={<AlignLeftOutlined />}
        active={editor?.isActive({ textAlign: 'left' })}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().setTextAlign('left').run(),
          )
        }
      />
      <ToolbarButton
        title="居中"
        icon={<AlignCenterOutlined />}
        active={editor?.isActive({ textAlign: 'center' })}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().setTextAlign('center').run(),
          )
        }
      />
      <ToolbarButton
        title="右对齐"
        icon={<AlignRightOutlined />}
        active={editor?.isActive({ textAlign: 'right' })}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().setTextAlign('right').run(),
          )
        }
      />

      <ToolbarDivider />

      <ToolbarButton
        title="清除格式"
        icon={<ClearOutlined />}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) =>
            currentEditor.chain().focus().unsetAllMarks().clearNodes().run(),
          )
        }
      />
      <ToolbarButton
        title="撤销"
        icon={<UndoOutlined />}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) => currentEditor.chain().focus().undo().run())
        }
      />
      <ToolbarButton
        title="重做"
        icon={<RedoOutlined />}
        disabled={commandDisabled}
        onClick={() =>
          run((currentEditor) => currentEditor.chain().focus().redo().run())
        }
      />

      {previewable && (
        <>
          <span className={styles.toolbarSpacer} />
          <ToolbarButton
            title={mode === 'preview' ? '退出预览' : '预览'}
            icon={
              mode === 'preview' ? <EyeInvisibleOutlined /> : <EyeOutlined />
            }
            active={mode === 'preview'}
            disabled={!editor}
            onClick={() =>
              onModeChange(mode === 'preview' ? 'edit' : 'preview')
            }
          />
        </>
      )}
    </div>
  );
};

const FroFormTiptap = ({
  value,
  defaultValue,
  editable = true,
  disabled = false,
  toolbar = true,
  previewable = true,
  placeholder = '请输入内容',
  minHeight = 180,
  maxHeight,
  extensions,
  className,
  style,
  onChange,
  onBlur,
}: FroFormTiptapProps) => {
  const { styles, cx } = useFroFormTiptapStyles();
  const [mode, setMode] = useState<'edit' | 'preview'>('edit');
  const isEditable = editable && !disabled;
  const mergedExtensions = useMemo(
    () => extensions ?? createFroFormTiptapExtensions(placeholder),
    [extensions, placeholder],
  );
  const editor = useEditor({
    extensions: mergedExtensions,
    content: normalizeHtml(value ?? defaultValue),
    editable: isEditable,
    immediatelyRender: false,
    onUpdate: ({ editor: currentEditor }) => {
      emitChange(currentEditor, onChange);
    },
    onBlur: () => {
      onBlur?.();
    },
  });
  const contentStyle = {
    '--fro-form-tiptap-min-height': toCssSize(minHeight),
    maxHeight: toCssSize(maxHeight),
    overflowY: maxHeight ? 'auto' : undefined,
  } as CSSProperties;

  useEffect(() => {
    editor?.setEditable(isEditable);
  }, [editor, isEditable]);

  useEffect(() => {
    if (!editor || value === undefined) {
      return;
    }

    const nextValue = normalizeHtml(value);

    if (nextValue !== editor.getHTML()) {
      editor.commands.setContent(nextValue, { emitUpdate: false });
    }
  }, [editor, value]);

  return (
    <div
      className={cx(styles.shell, disabled && styles.shellDisabled, className)}
      style={style}
    >
      {toolbar && (
        <FroFormTiptapToolbar
          editor={editor}
          disabled={!isEditable}
          previewable={previewable}
          mode={mode}
          onModeChange={setMode}
        />
      )}

      {mode === 'preview' ? (
        <FroFormTiptapPreview
          bordered={false}
          content={editor?.getHTML() ?? value}
          minHeight={minHeight}
          maxHeight={maxHeight}
        />
      ) : (
        <EditorContent
          editor={editor}
          className={styles.content}
          style={contentStyle}
        />
      )}
    </div>
  );
};

export default FroFormTiptap;
