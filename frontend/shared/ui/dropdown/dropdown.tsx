import { useState, useRef, useEffect, forwardRef, useImperativeHandle } from 'react';
import { Dropdown as DropdownBase, DropdownProps } from 'primereact/dropdown';

/**
 * Dropdown with width fix:
 *  https://github.com/primefaces/primereact/issues/5040
 *
 * TODO: check!
 */

export const Dropdown = forwardRef<any, DropdownProps>((props, ref) => {
  const internalRef = useRef<any>(null);
  const [currentWidth, setCurrentWidth] = useState<number | undefined>();

  // Expose internal ref to parent component if a ref is passed
  useImperativeHandle(ref, () => internalRef.current);

  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    if (internalRef.current) {
      const element = internalRef.current.getElement();
      if (element) {
        setCurrentWidth(element.clientWidth);
      }
    }
  }, [internalRef]);

  return (
    <DropdownBase
      ref={internalRef}
      {...props}
      pt={{
        ...props.pt,
        panel: {
          style: {
            // @ts-ignore
            ...(props.pt?.panel?.style ?? {}),
            ...(currentWidth ? { width: currentWidth } : {}),
          },
        },
      }}
    />
  );
});
