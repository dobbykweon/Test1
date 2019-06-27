#import "CaptureHelper.h" 
static CaptureHelper * captureHelper = [CaptureHelper sharedInstancs]; 
@implementation CaptureHelper : NSObject 

+ (void)initialize{ 
    if(captureHelper == nil) 
        captureHelper = [[CaptureHelper alloc] init]; 
} 

+ (CaptureHelper *)sharedInstancs{ 
    return captureHelper; 
} 

- (id)init
{ 
    if(captureHelper != nil){ 
        return captureHelper; 
    } 
    self = [super init]; 
    if(self){ 
        captureHelper = self; 
    } 
    return self; 
} 

- (NSString *)getDocumentDirectory { 
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES); 
    return [paths objectAtIndex:0]; 
} 
@end 
extern "C"{ 
    void CaptureToCameraRoll(const char *fileName) { 
        NSString *file = [NSString stringWithUTF8String:(fileName)]; 
        NSString *filePath = [[captureHelper getDocumentDirectory] stringByAppendingString:file]; 
        UIImage *image = [[UIImage alloc] initWithContentsOfFile:filePath]; 
        UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil); 
    }
}
