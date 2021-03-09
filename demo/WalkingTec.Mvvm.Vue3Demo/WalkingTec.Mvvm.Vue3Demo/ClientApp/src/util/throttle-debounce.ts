export const DEFAULT_DEBOUNCE_DURATION = 500;
export type Procedure = (...args: any[]) => any;

interface Index {
  (...args: any[]): any;
  clear: () => void;
  flush: () => void;
}
/**
 * 防抖
 * @param func
 * @param wait
 * @param immediate
 */
export const debounce = (
  func: Procedure,
  wait: number = DEFAULT_DEBOUNCE_DURATION,
  immediate: boolean = false
) => {
  let timeout: number | null;
  let args: any;
  let context: any;
  let result: any;

  const later = () => {
    timeout = null;
    if (!immediate) {
      result = func.apply(context, args);
      context = args = null;
    }
  };

  const debouncedFunc: Procedure = function(this: any) {
    context = this;
    args = arguments;
    const callNow = immediate && !timeout;

    if (!timeout) {
      timeout = window.setTimeout(later, wait);
    }

    if (callNow) {
      result = func.apply(context, args);
      context = args = null;
    }

    return result;
  };

  const clear = () => {
    if (timeout) {
      clearTimeout(timeout);
      timeout = null;
    }
  };

  const flush = () => {
    if (timeout) {
      result = func.apply(context, args);
      context = args = null;

      clearTimeout(timeout);
      timeout = null;
    }
  };

  const debounced: Index = (() => {
    const f: any = debouncedFunc;
    f.clear = clear;
    f.flush = flush;
    return f;
  })();

  return debounced;
};
