import clsx from 'clsx';
import { useStoreMap, useUnit } from 'effector-react';
import { Menu } from 'primereact/menu';
import React from 'react';
import { $activeItemsIds, $menuItems, NavMenuItem, navigate } from './model.ts';
import { Tag } from 'primereact/tag';

export const ProductNavbar = React.memo(() => {
  const items = useStoreMap($menuItems, (items) =>
    items.map((item) => ({ id: item.id, template: <NavbarItemTemplate {...item} /> })),
  );

  return (
    <nav>
      <Menu
        className="p-0 h-full"
        style={{ width: '368px' }}
        model={items}
        pt={{
          root: { style: { background: 'none', border: 'none' } },
          menu: { style: { background: 'none', border: 'none' } },
        }}
      />
    </nav>
  );
});

function NavbarItemTemplate(item: NavMenuItem) {
  const isActive = useStoreMap($activeItemsIds, (ids) => ids.includes(item.id!));

  return (
    <div
      className={clsx({
        'text-color-secondary': !isActive,
      })}
      onClick={() => {
        if (!item.disabled) {
          navigate(item.id!);
        }
      }}
    >
      {'parent' in item ? (
        <div className="p-menuitem-content">
          <a aria-current={isActive} className="p-menuitem-link p-1 pl-8 ml-3">
            <span className="text-sm">{item.label}</span>
          </a>
        </div>
      ) : (
        <a
          aria-current={isActive}
          className={clsx('p-menuitem-link', 'p-2', 'pl-7 font-semibold', item.className)}
          style={item.disabled || ('tag' in item && item.tag === 'soon') ? { cursor: 'default' } : undefined}
        >
          <span className={clsx('icon-2xl mr-2', item.disabled && 'surface-400', item.icon)} />
          <span className={clsx(item.disabled && 'opacity-40')}>{item.label}</span>
          {'tag' in item && (item.tag === 'new' || item.tag === 'soon') && (
            <Tag className="ml-3" severity={item.tag === 'soon' ? 'secondary' : undefined}>
              {item.tag === 'new' ? 'новое' : 'скоро'}
            </Tag>
          )}
        </a>
      )}
    </div>
  );
}
