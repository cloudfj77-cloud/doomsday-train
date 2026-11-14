import sys, time, asyncio
from playwright.async_api import async_playwright

targets = sys.argv[1:] or ['GroundZero&iso']


async def main():
    async with async_playwright() as p:
        b = await p.chromium.launch(channel='chrome', headless=True, args=['--enable-unsafe-swiftshader', '--ignore-gpu-blocklist'])
        pg = await b.new_page(viewport={'width': 1500, 'height': 880})
        logs = []
        pg.on('console', lambda m: logs.append(m.type + ': ' + m.text))
        pg.on('pageerror', lambda e: logs.append('pageerror: ' + str(e)))
        for t in targets:
            name = t.replace('&', '_').replace('#', '')
            if t.startswith('tab:'):
                await pg.goto('http://127.0.0.1:8765/explorer.html#Base')
                await pg.wait_for_timeout(1500)
                await pg.click(f'#tabs button:nth-child({int(t[4:])})')
                await pg.wait_for_timeout(4000)
            else:
                await pg.goto('about:blank')
                await pg.goto('http://127.0.0.1:8765/explorer.html#' + t)
                t0 = time.time()
                try:
                    await pg.wait_for_function("document.querySelector('#load').style.display==='none'", timeout=150000)
                except Exception as e:
                    print('timeout', t)
                await pg.wait_for_timeout(2500)
                print(t, 'loaded in', round(time.time() - t0, 1), 's', flush=True)
            await pg.screenshot(path=f'shots/{name}.png')
        print('\n'.join(logs[-15:]))
        await b.close()

asyncio.run(main())
