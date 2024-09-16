import * as dayjs from 'dayjs';

const ShortDateFormat = 'MMM DD, YYYY';
const ShortDateTimeFormat = 'MMM DD, YYYY hh:mm A';
const LongDateTimeFormat = 'MMM DD, YYYY hh:mm:ss A';
const ClientTimezone = 7;

export default class DateTimeService {
    static firstDayOfMonth(date) {
        return dayjs(date).startOf('month');
    }

    static lastDayOfMonth(date) {
        return dayjs(date).endOf('month');
    }

    static toShortDateFormat(date) {
        return dayjs(date).format(ShortDateFormat);
    }

    static toShortDateTimeFormat(date) {
        return dayjs(date).format(ShortDateTimeFormat);
    }

    static toLongDateTimeFormat(date) {
        return dayjs(date).format(LongDateTimeFormat);
    }

    static toClientTimezone(date) {
        return dayjs(date, LongDateTimeFormat).add(ClientTimezone, 'hour');
    }
}