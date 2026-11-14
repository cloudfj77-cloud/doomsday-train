public class EventListSpecial
{
	public static StoreEvent LotteryDraw()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "lottery_draw";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_lottery_draw_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_lottery_draw_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_lottery_draw_desc", PlayerStore.Instance.currentWinningLotteryNumber);
		storeEvent.duration = 1;
		storeEvent.importance = 7;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.LOWER;
		return storeEvent;
	}
}
