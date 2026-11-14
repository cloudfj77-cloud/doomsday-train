import os
from PIL import Image, ImageDraw, ImageFont
O = 'art/scene'
F = ImageFont.truetype('/System/Library/Fonts/Supplemental/Arial.ttf', 15)
FT = ImageFont.truetype('/System/Library/Fonts/Supplemental/Arial.ttf', 22)


def grid(paths, out, cell, cols, title, bg=(246, 245, 240)):
    rows = (len(paths) + cols - 1) // cols
    W, H = cols * cell, rows * (cell + 22) + 44
    img = Image.new('RGB', (W, H), bg)
    d = ImageDraw.Draw(img)
    d.text((12, 10), title, fill=(30, 30, 30), font=FT)
    for i, p in enumerate(paths):
        im = Image.open(p).convert('RGBA')
        im.thumbnail((cell - 10, cell - 10))
        x = (i % cols) * cell
        y = 44 + (i // cols) * (cell + 22)
        img.paste(Image.new('RGBA', (cell - 6, cell - 6), (30, 32, 36, 255)), (x + 3, y + 3))
        img.paste(im, (x + (cell - im.width) // 2, y + (cell - im.height) // 2), im)
        d.text((x + 6, y + cell), os.path.splitext(os.path.basename(p))[0].replace('MiniMap_', '')[:34], fill=(50, 50, 50), font=F)
    img.save(out, quality=90)
    print(out, img.size)


mm = sorted(os.listdir(O + '/minimap'))
grid([f'{O}/minimap/{f}' for f in mm], f'{O}/minimaps_all.jpg', 300, 6, f'All in-game minimaps ({len(mm)})')
ka = ['Title', 'Cover', 'Splash01', 'CoverLow', 'MainMenuBackground', 'Logo_schinese', 'Map_GroundZero', 'Map_Farm', 'Map_Hiddenwarehouse', 'Map_JLab_2', 'Map_StormZone', 'Map_Snow', 'Cover_GoldMiner', 'Cover_BubblePopper', 'Cover_MouseDodger', 'Cover_Island', 'TagillaPoster', 'TagillaPoster3', 'MapCrop', 'Splash']
grid([f'{O}/keyart/{f}.png' for f in ka if os.path.exists(f'{O}/keyart/{f}.png')], f'{O}/keyart_all.jpg', 300, 5, 'Key art / covers / map banners')
