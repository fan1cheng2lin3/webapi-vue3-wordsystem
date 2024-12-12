export const isNull = (data) => {
    if (data === null || data === undefined) return true; // null 或 undefined
    if (typeof data === "object" && Object.keys(data || {}).length === 0) return true; // 空对象
    if (Array.isArray(data) && data.length === 0) return true; // 空数组
    return false;
  };
  