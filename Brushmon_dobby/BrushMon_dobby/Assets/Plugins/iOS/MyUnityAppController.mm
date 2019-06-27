#import "UnityAppController.h"
#import "AVFoundation/AVFoundation.h"
 
@interface MyUnityAppController: UnityAppController {}
 
-(void)setAudioSession;
 
@end
 
@implementation MyUnityAppController
 
-(void) startUnity: (UIApplication*) application
{
    NSLog(@"MyUnityAppController startUnity");
    [super startUnity: application];  //call the super.
    [self setAudioSession];
}
 
- (void)setAudioSession
{
    NSLog(@"MyUnityAppController Set audiosession");
    AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback error:nil];
    [[AVAudioSession sharedInstance] setActive:YES error:nil];
}
 
@end
 
IMPL_APP_CONTROLLER_SUBCLASS(MyUnityAppController)