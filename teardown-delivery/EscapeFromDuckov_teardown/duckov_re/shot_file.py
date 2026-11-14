import asyncio, time
from playwright.async_api import async_playwright
U = 'file:///Users/cloud/WorkBuddy/spaces/2026_11_07_15-28-09/duckov_re/explorer.html'


async def m():
    async with async_playwright() as p:
        b = await p.chromium.launch(channel='chrome', headless=True)
        pg = await b.new_page(viewport={'width': 1500, 'height': 880}); logs = []
        pg.on('console', lambda x: logs.append(x.type + ': ' + x.text)); pg.on('pageerror', lambda e: logs.append('pageerror: ' + str(e)))
        for k in ['Desert', 'Base', 'Farm']:
            await pg.goto('about:blank'); t0 = time.time()
            await pg.goto(f'{U}#{k}')
            try:
                await pg.wait_for_function("document.querySelector('#load').style.display==='none'", timeout=120000)
            except Exception:
                print(k, 'NOT LOADED:', await pg.inner_text('#loadTxt'))
            await pg.wait_for_timeout(2000)
            print(k, 'file:// loaded in', round(time.time() - t0, 1), 's', flush=True)
            await pg.screenshot(path=f'shots/file_{k}.png')
        # switch scene by clicking in UI
        await pg.click('.sc[data-k="StormZone"]')
        await pg.wait_for_function("document.querySelector('#load').style.display==='none'", timeout=120000)
        await pg.wait_for_timeout(1500); await pg.screenshot(path='shots/file_click_StormZone.png'); print('click switch ok')
        await pg.click('#tabs button:nth-child(6)'); await pg.wait_for_timeout(3000)
        await pg.screenshot(path='shots/file_models.png'); print('models:', (await pg.inner_text('#mInfo'))[:60])
        print('\n'.join(l for l in logs if 'error' in l.lower())[-1500:] or 'no errors')
        await b.close()

asyncio.run(m())
