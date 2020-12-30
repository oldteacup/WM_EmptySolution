#pragma once
#ifdef EXPORTDEMO_H
#define DLL_DECLSPEC __declspec(dllexport)
#else
#define DLL_DECLSPEC __declspec(dllimport)
#endif

class DLL_DECLSPEC ExportDemo
{
	void ExternFunc2();
};

