import clsx from 'clsx';
import React, { ReactNode, CSSProperties } from 'react';
import styles from './layout.module.css';

export const Layout: React.FC<{
  header?: ReactNode;
  footer?: ReactNode;
  sidebar?: ReactNode;
  children: ReactNode;
  className?: string;
  mainClassName?: string;
  sidebarClassName?: string;
  sidebarStyle?: CSSProperties;
  mainStyle?: CSSProperties;
  style?: CSSProperties;
}> = ({
  header,
  children,
  footer,
  className,
  mainClassName,
  sidebarClassName,
  style,
  sidebar,
  sidebarStyle,
  mainStyle,
}) => {
  return (
    <>
      <div className={`${styles.layout} ${className || ''}`} style={style}>
        <header className="container">{header}</header>
        <div className={clsx(styles.content, { 'justify-content-center': sidebar == null })}>
          <aside className={clsx(sidebarClassName)} style={sidebarStyle}>
            {sidebar}
          </aside>
          <main className={clsx('container', mainClassName)} style={mainStyle}>
            {children}
          </main>
        </div>
        <footer>{footer}</footer>
      </div>
    </>
  );
};
