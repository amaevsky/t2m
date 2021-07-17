import useBreakpoint from "antd/lib/grid/hooks/useBreakpoint";
import { Breakpoint } from "antd/lib/_util/responsiveObserve";

export interface IHasBreakpoint {
  breakpoint: Partial<Record<Breakpoint, boolean>>
}

export const withBreakpoint = (Component: any) => {
  return () => {
    const breakpoint = useBreakpoint();
    return <Component breakpoint={ breakpoint } />
  }
}