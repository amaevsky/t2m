export const getIs12Hours = () => {
  var date = new Date();
  var dateString = date.toLocaleTimeString();

  return !!(dateString.match(/am|pm/i) || date.toString().match(/am|pm/i));
}

export const is12Hours = getIs12Hours();
export const DateFormat_DayOfWeek = 'ddd, MMM DD';
export const DateFormat = 'DD-MMM-YYYY';
export const TimeFormat = is12Hours ? 'h:mm A' : 'HH:mm';
export const DateTimeFormat = `${DateFormat} ${TimeFormat}`;