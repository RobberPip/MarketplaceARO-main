type SvgIconProps = {
  name: string;
  alt?: string;
  className?: string;
  width?: number | string;
  height?: number | string;
};

export function SvgIcon({ name, alt = '', className, width, height }: SvgIconProps) {
  const src = new URL(`../../assets/icons/${name}.svg`, import.meta.url).href;

  return <img src={src} alt={alt || name} className={className} width={width} height={height} />;
}
