//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace MainUiCpp;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::Devices::Input;
using namespace Windows::UI::Input;

#include <sstream>
#include <windows.h>

MainPage::MainPage()
{
	InitializeComponent();
}

void MainPage::TouchGrid_PointerPressed(Object^ sender, PointerRoutedEventArgs^ e)
{
	// Capture so we keep getting move events even if pointer leaves the Grid while held
	TouchGrid->CapturePointer(e->Pointer);

	auto point = e->GetCurrentPoint(TouchGrid);
	auto pos = point->Position;

	std::wstringstream ss;
	ss << L"[Down] Device=" << (int)point->PointerDevice->PointerDeviceType
	   << L" Id=" << point->PointerId
	   << L" X=" << pos.X
	   << L" Y=" << pos.Y
	   << L"\r\n";
	OutputDebugStringW(ss.str().c_str());
}

void MainPage::TouchGrid_PointerReleased(Object^ sender, PointerRoutedEventArgs^ e)
{
	c = 0;
	auto point = e->GetCurrentPoint(TouchGrid);
	auto pos = point->Position;

	std::wstringstream ss;
	ss << L"[Up]   Device=" << (int)point->PointerDevice->PointerDeviceType
	   << L" Id=" << point->PointerId
	   << L" X=" << pos.X
	   << L" Y=" << pos.Y
	   << L"\r\n";
	OutputDebugStringW(ss.str().c_str());

	TouchGrid->ReleasePointerCapture(e->Pointer);
}

void MainPage::TouchGrid_PointerMoved(Object^ sender, PointerRoutedEventArgs^ e)
{
	auto point = e->GetCurrentPoint(TouchGrid);

	// Only log while a button / contact is actually down (dragging)
	bool isDragging = false;

	auto pointerDeviceType = point->PointerDevice->PointerDeviceType;
	switch (pointerDeviceType)
	{
	case PointerDeviceType::Touch:
	case PointerDeviceType::Pen:
		isDragging = point->IsInContact; // finger/stylus must be pressed
		break;
	case PointerDeviceType::Mouse:
	{
		auto props = point->Properties;
		isDragging =
			props->IsLeftButtonPressed ||
			props->IsMiddleButtonPressed ||
			props->IsRightButtonPressed ||
			props->IsXButton1Pressed ||
			props->IsXButton2Pressed;
		break;
	}
	default:
		break;
	}

	if (!isDragging)
		return;

	auto pos = point->Position;

	std::wstringstream ss;
	ss << L"[Move]["<<++c<<"] Device = " << (int)point->PointerDevice->PointerDeviceType
	   << L" Id=" << point->PointerId
	   << L" X=" << pos.X
	   << L" Y=" << pos.Y
	   << L"\r\n";
	OutputDebugStringW(ss.str().c_str());
}
