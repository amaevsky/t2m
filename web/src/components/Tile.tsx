export const Tile = (props: any) => {
  return (
    <div {...props} className="tile">
      {props.children}
    </div>
  );
}